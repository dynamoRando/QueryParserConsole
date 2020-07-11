using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


public class SelectQueryPlanGenerator
{
    #region Private Fields
    SelectStatement _statement;
    int _level = 0;
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
        _statement = statement;
        var plan = new QueryPlan();

        var endStatements = GetEndStatements(statement.Statements);
        var searchEndSteps = GetSearchParts(endStatements);
        var booleanSteps = GetBooleanSteps(searchEndSteps);

        throw new NotImplementedException();
    }
    #endregion

    #region Private Methods
    private List<SearchStep> GetSearchParts(List<StatementPart> parts)
    {
        var result = new List<SearchStep>();
        foreach (var part in parts)
        {
            var step = new SearchStep(part);
            step.Level = _level;
            result.Add(step);
        }

        return result;
    }

    private List<StatementPart> GetEndStatements(List<StatementPart> parts)
    {
        var result = new List<StatementPart>();

        foreach (var statement in parts)
        {
            if (!statement.Text.Contains("("))
            {
                result.Add(statement);
            }
        }

        return result;
    }

    private List<BoolStep> GetBooleanSteps(List<SearchStep> steps)
    {
        var result = new List<BoolStep>();
        _level++;

        foreach (var step in steps)
        {
            BoolStep boolStep = null;
            boolStep = GetBoolStepFromStep(step, " AND ", steps);
            if (boolStep != null)
            {
                if (IsValidBoolStep(boolStep))
                {
                    result.Add(boolStep);
                }
                
            }
            else
            {
                boolStep = GetBoolStepFromStep(step, " OR ", steps);
                if (boolStep != null)
                {
                    if (IsValidBoolStep(boolStep))
                    {
                        result.Add(boolStep);
                    }
                }
            }
        }

        return result;
    }

    private bool IsValidBoolStep(BoolStep step)
    {
        if (step != null)
        {
            if (step.InputOne != null && step.InputTwo != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private BoolStep GetBoolStepFromStep(SearchStep step, string boolTerm, List<SearchStep> steps)
    {
        
        var stepParentText = step.Part.StatementParentWithWhiteSpace;
        var stepGrandParentText = step.Part.StatementGrandParentWithWhiteSpace;
        BoolStep boolStep = null;
        
        if (stepParentText.Contains(boolTerm))
        {
            var boolstep = new BoolStep();
            boolstep.Level = _level;
            boolstep.Boolean = boolTerm.Trim();
            boolstep.InputOne = step;
            int indexOfBool = stepParentText.IndexOf(boolTerm);

            // need to find the other half of the AND statement
            var otherTermText = stepParentText.Substring(indexOfBool);
            var otherTerm = steps.Where(s => s.Part.TextWithWhiteSpace.Equals(otherTermText)).FirstOrDefault();
            if (otherTerm != null)
            {
                // make sure we don't already have a boolstep for ourselves
                if (otherTerm.Part.Text != step.Part.Text)
                {
                    boolstep.InputTwo = otherTerm;
                }
                else
                {
                    return null;
                }
            }
        }

        if (stepParentText.Equals(step.Part.TextWithWhiteSpace))
        { 

        }

        return boolStep;
    }

    #endregion
}
