﻿using System;
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

        plan.Steps.AddRange(searchEndSteps);
        plan.Steps.AddRange(booleanSteps);

        return plan;
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

        // if the previous step has a BOOLEAN and is not part of a () grouping
        if (stepParentText.Contains(boolTerm) && !stepParentText.Contains("("))
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

        // if the previous step has a boolean AND is part of a () grouping
        // AGE > 32
        if (stepParentText.Contains(boolTerm) && stepParentText.Contains("("))
        {
            var clauses = stepParentText.Split('(', ')').ToList();
            var nextStep = new BoolStep();
            var lastBoolStep = new BoolStep();
            foreach(var clause in clauses)
            {
                foreach(var b in boolSteps)
                {
                    if (b.BoolStepTextWithWhiteSpace.Contains(clause))
                    {
                        lastBoolStep = b;
                        nextStep.InputOne = b;
                        nextStep.InputTwo = step;
                        break;
                    }
                }
            }

            var parse = stepParentText;
            parse = parse.Remove(parse.IndexOf(lastBoolStep.BoolStepTextWithWhiteSpace), lastBoolStep.BoolStepTextWithWhiteSpace.Length);
            parse = parse.Remove(parse.IndexOf(step.Part.TextWithWhiteSpace), step.Part.TextWithWhiteSpace.Length);
            if (parse.Contains(" AND "))
            {
                nextStep.Boolean = "AND";
            }
            if (parse.Contains(" OR "))
            {
                nextStep.Boolean = "OR";
            }
            nextStep.Level = _level++;
            nextStep.BoolStepTextWithWhiteSpace = stepParentText;
            boolStep = nextStep;
        }

        // if the previous step is part of a multi BOOLEAN (i.e. NAME = MEGAN)
        if (stepParentText.Equals(step.Part.TextWithWhiteSpace) && !stepGrandParentText.Equals(step.Part.TextWithWhiteSpace))
        {
            var totalBools = 0;
            var words = stepGrandParentText.Split(" ").ToList();
            foreach (var word in words)
            {
                if (word.Equals("AND") || word.Equals("OR"))
                {
                    totalBools++;
                }
            }

            if (totalBools > 1)
            {
                // we know we are part of a multi BOOL operation at the same level and need to 
                // find the previous bool operator(s) for the other terms
                foreach (var b in boolSteps)
                {
                    if (b.InputOne is SearchStep && b.InputTwo is SearchStep)
                    {
                        var input1 = (b.InputOne as SearchStep);
                        var input2 = (b.InputTwo as SearchStep);
                        if (stepGrandParentText.Contains(input1.Part.TextWithWhiteSpace) &&
                            stepGrandParentText.Contains(input2.Part.TextWithWhiteSpace))
                        {
                            // we need to link this boolstep to the next planstep
                            var nextStep = new BoolStep();
                            nextStep.Level = _level++;
                            nextStep.InputOne = b;
                            nextStep.InputTwo = step;
                            var parse = stepGrandParentText;
                            parse = parse.Remove(parse.IndexOf(input1.Part.TextWithWhiteSpace), input1.Part.TextWithWhiteSpace.Length);
                            parse = parse.Remove(parse.IndexOf(b.Boolean), b.Boolean.Length);
                            parse = parse.Remove(parse.IndexOf(input2.Part.TextWithWhiteSpace), input2.Part.TextWithWhiteSpace.Length);
                            parse = parse.Remove(parse.IndexOf(step.Part.TextWithWhiteSpace), step.Part.TextWithWhiteSpace.Length);
                            if (parse.Contains(" AND "))
                            {
                                nextStep.Boolean = "AND";
                            }
                            if (parse.Contains(" OR "))
                            {
                                nextStep.Boolean = "OR";
                            }
                            nextStep.BoolStepTextWithWhiteSpace = stepGrandParentText;
                            boolStep = nextStep;
                            break;
                        }
                    }
                }
            }
        }

        // we are an outermost term
        // NAME = BRIAN
        if (stepParentText.Equals(stepGrandParentText))
        {
            int maxLevel = boolSteps.Max(i => i.Level);
            var maxStep = boolSteps.Where(i => i.Level == maxLevel).FirstOrDefault();
            if (maxStep != null)
            {
                boolStep = new BoolStep();
                boolStep.InputOne = maxStep;
                boolStep.InputTwo = step;
                boolStep.Level = maxLevel++;
            }
        }

        return boolStep;
    }

    #endregion
}
