using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


public class SelectQueryPlanGenerator
{
    #region Private Fields
    SelectStatement _statement;
    #endregion

    #region Public Properties
    #endregion

    #region Protected Methods
    #endregion

    #region Events
    #endregion

    #region Constructors
    #endregion

    #region Public Methods
    public QueryPlan GeneratePlan(SelectStatement statement)
    {
        var plan = new QueryPlan();
        _statement = statement;

        var endStatements = GetEndStatements();

        throw new NotImplementedException();
    }
    #endregion

    #region Private Methods
    private List<StatementPart> GetEndStatements()
    {
        var result = new List<StatementPart>();

        foreach(var statement in _statement.Statements)
        {
            if (!statement.Text.Contains("("))
            {
                result.Add(statement);
            }
        }

        return result;
    }

    private List<BoolStep> GetBooleanSteps(List<StatementPart> endStatements)
    {
        var result = new List<BoolStep>();

        throw new NotImplementedException();

        return result;
    }

    #endregion
}
