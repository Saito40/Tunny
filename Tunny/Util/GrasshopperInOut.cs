using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using GalapagosComponents;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;

using Rhino.FileIO;

using Tunny.Component;
using Tunny.UI;

namespace Tunny.Util
{
    public class GrasshopperInOut
    {
        private readonly GH_Document _document;
        private readonly List<Guid> _inputGuids;
        private readonly TunnyComponent _component;
        private List<GalapagosGeneListObject> _genePool;
        private List<IGH_Param> _geometries;
        public List<IGH_Param> Objectives;
        public List<GH_NumberSlider> Sliders;

        public readonly string ComponentFolder;
        public List<Variable> Variables;
        public string DocumentPath;
        public string DocumentName;

        public GrasshopperInOut(TunnyComponent component)
        {
            _component = component;
            ComponentFolder = Path.GetDirectoryName(Grasshopper.Instances.ComponentServer.FindAssemblyByObject(_component).Location);
            _document = _component.OnPingDocument();
            _inputGuids = new List<Guid>();
            SetInputs();
        }

        private bool SetInputs()
        {
            SetVariables();
            SetObjectives();
            SetModelGeometries();

            return true;
        }

        private bool SetVariables()
        {
            Sliders = new List<GH_NumberSlider>();
            _genePool = new List<GalapagosGeneListObject>();

            foreach (IGH_Param source in _component.Params.Input[0].Sources)
            {
                _inputGuids.Add(source.InstanceGuid);
            }

            if (_inputGuids.Count == 0)
            {
                TunnyMessageBox.Show("No input variables found. Please connect a number slider to the input of the component.", "Tunny");
                return false;
            }

            foreach (IGH_DocumentObject input in _inputGuids.Select(guid => _document.FindObject(guid, true)))
            {
                switch (input)
                {
                    case GH_NumberSlider slider:
                        Sliders.Add(slider);
                        break;
                    case GalapagosGeneListObject genePool:
                        _genePool.Add(genePool);
                        break;
                }
            }

            var variables = new List<Variable>();
            SetInputSliderValues(variables);
            SetInputGenePoolValues(variables);
            Variables = variables;
            return true;
        }

        private void SetInputSliderValues(ICollection<Variable> variables)
        {
            int i = 0;

            foreach (GH_NumberSlider slider in Sliders)
            {
                decimal min = slider.Slider.Minimum;
                decimal max = slider.Slider.Maximum;

                decimal lowerBond;
                decimal upperBond;
                bool isInteger;
                string nickName = slider.NickName;
                if (nickName == "")
                {
                    nickName = "param" + i++;
                }

                switch (slider.Slider.Type)
                {
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Even:
                        lowerBond = min / 2;
                        upperBond = max / 2;
                        isInteger = true;
                        break;
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Odd:
                        lowerBond = (min - 1) / 2;
                        upperBond = (max - 1) / 2;
                        isInteger = true;
                        break;
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Integer:
                        lowerBond = min;
                        upperBond = max;
                        isInteger = true;
                        break;
                    default:
                        lowerBond = min;
                        upperBond = max;
                        isInteger = false;
                        break;
                }

                variables.Add(new Variable(lowerBond, upperBond, isInteger, nickName));
            }
        }

        private void SetInputGenePoolValues(ICollection<Variable> variables)
        {
            int count = 0;

            foreach (GalapagosGeneListObject genePool in _genePool)
            {
                bool isInteger = genePool.Decimals == 0;
                decimal lowerBond = genePool.Minimum;
                decimal upperBond = genePool.Maximum;

                for (int j = 0; j < genePool.Count; j++)
                {
                    string nickName = "genepool" + count++;
                    variables.Add(new Variable(lowerBond, upperBond, isInteger, nickName));
                }
            }
        }


        private bool SetObjectives()
        {
            if (_component.Params.Input[1].SourceCount == 0)
            {
                TunnyMessageBox.Show("No objective found. Please connect a number to the objective of the component.", "Tunny");
                return false;
            }

            Objectives = _component.Params.Input[1].Sources.ToList();
            return true;
        }

        private bool SetModelGeometries()
        {
            if (_component.Params.Input[2].SourceCount == 0)
            {
                return false;
            }

            _geometries = _component.Params.Input[2].Sources.ToList();
            return true;
        }

        private bool SetSliderValues(IList<decimal> parameters)
        {
            int i = 0;

            foreach (GH_NumberSlider slider in Sliders)
            {
                if (slider == null)
                {
                    return false;
                }
                decimal val;

                switch (slider.Slider.Type)
                {
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Even:
                        val = (int)parameters[i++] * 2;
                        break;
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Odd:
                        val = (int)(parameters[i++] * 2) + 1;
                        break;
                    case Grasshopper.GUI.Base.GH_SliderAccuracy.Integer:
                        val = (int)parameters[i++];
                        break;
                    default:
                        val = parameters[i++];
                        break;
                }

                slider.Slider.RaiseEvents = false;
                slider.SetSliderValue(val);
                slider.ExpireSolution(false);
                slider.Slider.RaiseEvents = true;
            }

            foreach (GalapagosGeneListObject genePool in _genePool)
            {
                for (int j = 0; j < genePool.Count; j++)
                {
                    genePool.set_NormalisedValue(j, GetNormalisedGenePoolValue(parameters[i++], genePool));
                    genePool.ExpireSolution(false);
                }
            }

            return true;
        }

        private decimal GetNormalisedGenePoolValue(decimal unnormalized, GalapagosGeneListObject genePool)
        {
            return (unnormalized - genePool.Minimum) / (genePool.Maximum - genePool.Minimum);
        }

        private void Recalculate()
        {
            while (_document.SolutionState != GH_ProcessStep.PreProcess || _document.SolutionDepth != 0) { }

            _document.NewSolution(true);

            while (_document.SolutionState != GH_ProcessStep.PostProcess || _document.SolutionDepth != 0) { }
        }

        public void NewSolution(IList<decimal> parameters)
        {
            SetSliderValues(parameters);
            Recalculate();
        }

        public List<double> GetObjectiveValues()
        {
            var values = new List<double>();

            foreach (IGH_Param objective in Objectives)
            {
                IGH_StructureEnumerator ghEnumerator = objective.VolatileData.AllData(false);
                if (ghEnumerator.Count() > 1)
                {
                    TunnyMessageBox.Show(
                        "Tunny doesn't handle list output.\n Separate each objective if you want multiple objectives",
                        "Tunny",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return new List<double>();
                }
                foreach (IGH_Goo goo in ghEnumerator)
                {
                    if (goo is GH_Number num)
                    {
                        values.Add(num.Value);
                    }
                    else if (goo == null)
                    {
                        values.Add(double.NaN);
                    }
                }
            }

            return values;
        }

        public List<string> GetGeometryJson()
        {
            var json = new List<string>();
            var option = new SerializationOptions();

            if (_geometries.Count == 0)
            {
                return json;
            }

            foreach (IGH_Param param in _geometries)
            {
                IGH_StructureEnumerator ghEnumerator = param.VolatileData.AllData(true);

                foreach (IGH_Goo goo in ghEnumerator)
                {
                    AddEachGHParamToJson(json, option, goo);
                }
            }

            return json;
        }

        private static void AddEachGHParamToJson(List<string> json, SerializationOptions option, IGH_Goo goo)
        {
            switch (goo)
            {
                case GH_Mesh mesh:
                    json.Add(mesh.Value.ToJSON(option));
                    break;
                case GH_Brep brep:
                    json.Add(brep.Value.ToJSON(option));
                    break;
                case GH_Curve curve:
                    json.Add(curve.Value.ToJSON(option));
                    break;
                case GH_Surface surface:
                    json.Add(surface.Value.ToJSON(option));
                    break;
                case GH_SubD subd:
                    json.Add(subd.Value.ToJSON(option));
                    break;
                default:
                    throw new Exception("Tunny doesn't handle this type of geometry");
            }
        }
    }
}
