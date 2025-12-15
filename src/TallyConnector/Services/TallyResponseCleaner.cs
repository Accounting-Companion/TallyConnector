using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TallyConnector.Services
{
    /// <summary>
    /// A TextReader that cleans invalid Tally characters on the fly.
    /// 1. Removes "&#4; " sequence.
    /// 2. Escapes ASCII control characters (0x00-0x1F) to &#Code; representation.
    /// </summary>
    public class TallyResponseCleaner : TextReader
    {
        private readonly TextReader _innerReader;
        private const string FilterSequence = "&#4; ";
        
        // Input lookahead buffer for detecting sequences to remove
        private readonly char[] _lookaheadBuffer;
        private int _lookaheadCount;

        // Output buffer for character expansion (e.g. \n -> &#10;)
        private readonly Queue<char> _outputQueue;

        public TallyResponseCleaner(TextReader innerReader)
        {
            _innerReader = innerReader;
            _lookaheadBuffer = new char[FilterSequence.Length];
            _lookaheadCount = 0;
            _outputQueue = new Queue<char>();
        }

        public override int Peek()
        {
            if (_outputQueue.Count > 0) return _outputQueue.Peek();
            
            // If output queue is empty, we must peek the next *logical* character.
            // This is complex because the next raw char might be a Control Char that expands,
            // or start of "&#4; " that disappears.
            // For simple usage (XmlReader), Peek() is rarely used extensively or can be imperfect.
            // But to be correct:
            if (_lookaheadCount > 0) return _lookaheadBuffer[0];
            return _innerReader.Peek();
        }

        public override int Read()
        {
            // 1. Draining Output Queue (Expansions)
            if (_outputQueue.Count > 0)
            {
                return _outputQueue.Dequeue();
            }

            // 2. Fetch Next Valid Raw Char
            int ch = GetNextRawChar();
            
            // Loop to handle "Removal" of sequences (skipping)
            while (ch != -1)
            {
                // Check for Removal Sequence Start ('&')
                if (ch == FilterSequence[0]) 
                {
                   if (TryMatchFilterSequence())
                   {
                       // Match found! Sequence discarded. Get next char.
                       ch = GetNextRawChar();
                       continue;
                   }
                }

                // Check for Control Char Escaping (0x00 - 0x1F)
                if (ch >= 0x00 && ch <= 0x1F)
                {
                    string replacement = $"&#{(int)ch};";
                    // Push all but first char to queue
                    for (int i = 1; i < replacement.Length; i++)
                    {
                        _outputQueue.Enqueue(replacement[i]);
                    }
                    return replacement[0];
                }

                // Valid regular char
                return ch;
            }

            return -1; // EOF
        }

        private int GetNextRawChar()
        {
            if (_lookaheadCount > 0)
            {
                char ch = _lookaheadBuffer[0];
                ShiftLookahead();
                return ch;
            }
            return _innerReader.Read();
        }

        private bool TryMatchFilterSequence()
        {
            // We have already seen/consumed the first char '&' via GetNextRawChar.
            // But we need to verify the rest of the sequence.
            // We must peek/read ahead. If mismatch, we must retain those chars in _lookaheadBuffer.

            // Since we consumed '&', if we fail, we can't "put it back" easily into _lookahead unless we treat it specially.
            // PROBLEM: GetNextRawChar() *returned* the '&'.
            // If we fail match, we must ensure that '&' is returned to the caller of Read().
            // But Read() has it in `ch`. If TryMatchFilterSequence returns false, Read() just proceeds to handle `ch` ('&') normally.
            // The constraint is: We need to pull N chars from source into _lookaheadBuffer to check them.

            // 1. Fill _lookaheadBuffer with needed chars check
            // Sequence length is 5. We consumed 1. Need 4 more.
            // We append to _lookaheadBuffer.
            // Current _lookaheadCount should be 0 because GetNextRawChar clears it before reading inner.
            
            // Error in Logic: GetNextRawChar takes from buffer. If buffer had data, we shouldn't be reading inner for sequence check yet?
            // Wait, if _lookaheadCount > 0, GetNextRawChar returned buffer[0].
            // Does buffer contain valid subsequent chars? Maybe.
            // Let's assume naive approach:
            // We need to verify `FilterSequence[1..end]` against next chars.
            
            // Simplified Read-Ahead:
            int matchIndex = 1;
            int bufferStartIndex = _lookaheadCount; // Start appending after existing lookahead?
            // Actually, if we are in flow, _lookaheadCount should be 0 if we hit inner reader.
            // If we hit lookahead, we might have partials?
            // Let's rely on a helper that fills lookahead.

            // We need to peek/read (FilterSequence.Length - 1) chars.
            // We will store them in _lookaheadBuffer *sequentially*.
            
            for (int i = 1; i < FilterSequence.Length; i++)
            {
                // We need to read into buffer at index `i-1` (since we consumed 0).
                // But wait, `_lookaheadBuffer` is a circular buffer or just a list?
                // `GetNextRawChar` consumes from [0].
                // So if we push to [0], [1]...
                
                // Let's just Read from InnerReader into `_lookaheadBuffer` if empty.
                // If `_lookaheadCount` has items, we check them first.
                
                int nextCh;
                if (_lookaheadCount > (i - 1)) 
                {
                     // We already have this char in buffer?
                     // No, `_lookaheadCount` decrements on consume.
                     // We are filling UP the buffer.
                     // This logic is getting complex given the mix of buffer consumption and filling.
                     
                     // Fallback: simpler logic.
                     // The Sequence is specific: "&#4; ".
                     // Just use a temp buffer for matching. If fail, push temp back to _lookahead.
                }
            }
            
            // RE-DESIGN TryMatchFilterSequence:
            // Goal: Match next 4 chars.
            // Source: Next raw chars.
            // Storage on fail: _lookaheadBuffer.
            
            var tempBuffer = new List<char>();
            bool match = true;
            
            for (int i = 1; i < FilterSequence.Length; i++)
            {
                int c = _innerReader.Read(); // Always read raw inner? What if lookahead had stuff?
                // Lookahead only has stuff if we backtracked before.
                // Since we assume simple stream, lookahead is empty usually.
                // But if we had partial match inside partial match? "&#&#4; " -> unlikely.
                
                if (c == -1) // EOF
                {
                    match = false;
                    break;
                }
                
                tempBuffer.Add((char)c);
                if ((char)c != FilterSequence[i])
                {
                    match = false;
                    // Don't break immediately, we need to read enough? No, we can stop matching but need to save chars.
                    // Actually if mismatch, stop matching, save `c`.
                    break;
                }
            }
            
            if (match)
            {
                // Consumed entire sequence successfully.
                return true;
            }
            else
            {
                // Mismatch. All chars in tempBuffer must be saved to _lookahead.
                // WE MUST PREPEND or APPEND?
                // `GetNextRawChar` takes from 0.
                // So we should expect these chars to be next.
                // If `_lookaheadBuffer` was empty, we put them at 0.
                // Copy tempBuffer to _lookaheadBuffer.
                for (int k = 0; k < tempBuffer.Count; k++)
                {
                    _lookaheadBuffer[_lookaheadCount++] = tempBuffer[k]; 
                }
                return false;
            }
        }

        private void ShiftLookahead()
        {
            if (_lookaheadCount > 0)
            {
                Array.Copy(_lookaheadBuffer, 1, _lookaheadBuffer, 0, _lookaheadCount - 1);
                _lookaheadCount--;
            }
        }

        public override int Read(char[] buffer, int index, int count)
        {
            int read = 0;
            for (int i = 0; i < count; i++)
            {
                int ch = Read();
                if (ch == -1) break;
                buffer[index + i] = (char)ch;
                read++;
            }
            return read;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerReader.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
