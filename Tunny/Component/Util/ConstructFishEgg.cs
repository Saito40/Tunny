﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using Tunny.Component.Params;
using Tunny.Input;
using Tunny.Type;
using Tunny.Util;

namespace Tunny.Component.Util
{
    public partial class ConstructFishEgg : GH_Component
    {
        private readonly Dictionary<string, FishEgg> _fishEggs = new Dictionary<string, FishEgg>();
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public ConstructFishEgg()
          : base("Construct Fish Egg", "ConstrFEgg",
            "You can specify the initial individual that Tunny will run. Try your golden egg!",
            "Tunny", "Util")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Variables", "Vars", "Variables pair to enqueue optimize.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Lay Egg", "Lay", "If true, add an egg", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Clear", "Clear", "If true, clear eggs", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_FishEgg(), "FishEgg", "FishEgg", "These eggs are enqueued for optimization and become fish.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool lay = false;
            bool clear = false;
            if (!DA.GetData(1, ref lay)) { return; }
            if (!DA.GetData(2, ref clear)) { return; }

            if (clear)
            {
                _fishEggs.Clear();
                return;
            }

            if (lay)
            {
                LayFishEgg();
            }

            DA.SetData(0, _fishEggs);
        }

        private void LayFishEgg()
        {
            var ghIO = new GrasshopperInOut(this, getVariableOnly: true);
            List<VariableBase> variables = ghIO.Variables;

            bool isContainedVariableSets = false;
            if (_fishEggs.Count > 0)
            {
                isContainedVariableSets = CheckVariableSetsIsContained(variables);
            }

            if (!isContainedVariableSets)
            {
                AddVariablesToFishEgg(variables);
            }
        }

        private bool CheckVariableSetsIsContained(IEnumerable<VariableBase> variables)
        {
            int sameValueCount = 0;
            foreach (VariableBase variable in variables)
            {
                string name = variable.NickName;
                switch (variable)
                {
                    case NumberVariable number:
                        if (_fishEggs.TryGetValue(name, out FishEgg eggNum) && eggNum.Values.Contains(number.Value))
                        {
                            sameValueCount++;
                        }
                        break;
                    case CategoricalVariable category:
                        if (_fishEggs.TryGetValue(name, out FishEgg eggCat) && eggCat.Category == category.SelectedItem)
                        {
                            sameValueCount++;
                        }
                        continue;
                }
            }
            bool isContainVariableSets = sameValueCount == _fishEggs.Count;
            return isContainVariableSets;
        }

        private void AddVariablesToFishEgg(IEnumerable<VariableBase> variables)
        {
            foreach (VariableBase variable in variables)
            {
                string name = variable.NickName;
                switch (variable)
                {
                    case NumberVariable number:
                        if (_fishEggs.TryGetValue(name, out FishEgg eggNum))
                        {
                            eggNum.Values.Add(number.Value);
                        }
                        else
                        {
                            _fishEggs.Add(name, new FishEgg(number));
                        }
                        break;
                    case CategoricalVariable category:
                        if (_fishEggs.TryGetValue(name, out FishEgg eggCat))
                        {
                            string message = $"This Categorical variable {name} is already contained in fish egg.";
                            TLog.Error(message);
                            throw new ArgumentException(message);
                        }
                        else
                        {
                            _fishEggs.Add(name, new FishEgg(category));
                        }
                        break;
                }
            }
        }

        public override void CreateAttributes()
        {
            m_attributes = new ConstructFishEggAttributes(this);
        }

        protected override System.Drawing.Bitmap Icon => Resources.Resource.ConstructFishEgg;
        public override Guid ComponentGuid => new Guid("00CC0C86-687F-4A28-93CF-30A1E361A7D5");
    }
}
