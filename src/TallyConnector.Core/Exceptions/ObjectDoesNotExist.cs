﻿namespace TallyConnector.Core.Exceptions;

public class ObjectDoesNotExist : Exception
{
    public ObjectDoesNotExist()
    {
    }

    public ObjectDoesNotExist(string message) : base(message)
    {
    }
    public ObjectDoesNotExist(string objectType, string identifier, string identifiervalue, string companyname) :
        base(companyname != null ? $"{objectType} with {identifier} - \"{identifiervalue}\" in \"{companyname}\" does not exist" : $"{objectType} with {identifier} - \"{identifiervalue}\" does not exist in active company in Tally")
    {
    }

    public ObjectDoesNotExist(string message, Exception innerException) : base(message, innerException)
    {
    }

}

