﻿namespace TallyConnector.Core.Models.Masters;

public interface IBaseTallyObject : IBaseObject
{
    string GUID { get; set; }

    string RemoteId { get; set; }
}

