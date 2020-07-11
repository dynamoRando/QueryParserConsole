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
            boolStep = GetBoolStepFromStep(step, " AND ", steps, result);
            if (boolStep != null)
            {
                if (IsValidBoolStep(boolStep))
                {
                    result.Add(boolStep);
                }
                
            }
            else
            {
                boolStep = GetBoolStepFromStep(step, " OR ", steps, result);
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

    private BoolStep GetBoolStepFromStep(SearchStep step, string boolTerm, List<SearchStep> steps, List<BoolStep> boolSteps)
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

            // need to find the other half of the BOOL statement
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
            var totalBools = 0;
            var words = stepGrandParentText.Split(" ").ToList();
            foreach(var word in words)
            {
                if (word.Equals("AND") || word.Equals("OR"))
                {
                    totalBools++;
                }
            }

            if (totalBools > 1)
            {
                // we know we are part of a multi BOOL operation and need to 
                // find the previous bool operator(s) for the other terms
                foreach(var b in boolSteps)
                {
                    if (b.InputOne is SearchStep && b.InputTwo is SearchStep)
                    {
                        var input1 = (b.InputOne as SearchStep);
                        var input2 = (b.InputTwo as SearchStep);
                        if (stepGrandParentText.Contains(input1.Part.TextWithWhiteSpace) &&
                            stepGrandParentText.Contains(input2.Part.TextWithWhiteSpace))
                        {
                            // we need to link this boolstep to 
                        }
                    }
                }
            }
        }

        return boolStep;
    }

    #endregion
}
