using System;
using System.Collections.Generic;
using System.Text;


    public class JoinStep : IPlanStep
    {
        #region Public Properties
        public Guid Id { get; set; }
        public int Level { get; set; }
        public PlanStep InputOne { get; set; }
        public PlanStep InputTwo { get; set; }
        public string Boolean { get; set; }
        #endregion

        #region Constructors
        #endregion

        #region Public Methods
        public PlanResult GetResult()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
