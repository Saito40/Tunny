using System;

using Serilog.Events;

using Tunny.Core.Enum;
using Tunny.Settings.Sampler;
using Tunny.Util;

namespace Tunny.UI
{
    public partial class OptimizationWindow
    {
        private void TpeDefaultButton_Click(object sender, EventArgs e)
        {
            TLog.MethodStart();
            SetTpeSettings(new Tpe());
        }

        private void Nsga2MutationProbCheckedChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            nsga2MutationProbUpDown.Enabled = nsga2MutationProbCheckBox.Checked;
        }

        private void Nsga2CrossoverCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            nsga2CrossoverComboBox.Enabled = nsga2CrossoverCheckBox.Checked;
        }

        private void Nsga2DefaultButton_Click(object sender, EventArgs e)
        {
            TLog.MethodStart();
            SetNsga2Settings(new NSGAII());
        }

        private void Nsga3MutationProbCheckedChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            nsga3MutationProbUpDown.Enabled = nsga3MutationProbCheckBox.Checked;
        }

        private void Nsga3CrossoverCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            nsga3CrossoverComboBox.Enabled = nsga3CrossoverCheckBox.Checked;
        }

        private void Nsga3DefaultButton_Click(object sender, EventArgs e)
        {
            TLog.MethodStart();
            SetNsga3Settings(new NSGAIII());
        }

        private void CmaEsSigmaCheckedChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            cmaEsSigmaNumUpDown.Enabled = cmaEsSigmaCheckBox.Checked;
        }

        private void CmaEsRestartStrategyCheckedChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            cmaEsIncPopSizeUpDown.Enabled = cmaEsRestartCheckBox.Checked;
            cmaEsPopulationSizeUpDown.Enabled = cmaEsRestartCheckBox.Checked;
        }

        private void CmaEsDefaultButton_Click(object sender, EventArgs e)
        {
            TLog.MethodStart();
            SetCmaEsSettings(new CmaEs());
        }

        private void CmaEsWarmStartCmaEsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            cmaEsWarmStartComboBox.Enabled = cmaEsWarmStartCmaEsCheckBox.Checked;
            cmaEsSigmaCheckBox.Enabled = !cmaEsWarmStartCmaEsCheckBox.Checked;
            cmaEsUseSaparableCmaCheckBox.Enabled = !cmaEsWarmStartCmaEsCheckBox.Checked;
        }

        private void CmaEsWithMarginCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            cmaEsUseSaparableCmaCheckBox.Enabled = !cmaEsWithMarginCheckBox.Checked;
        }

        private void CmaEsUseSaparableCmaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            cmaEsWithMarginCheckBox.Enabled = !cmaEsUseSaparableCmaCheckBox.Checked;
        }

        private void BoTorchDefaultButton_Click(object sender, EventArgs e)
        {
            TLog.MethodStart();
            SetBoTorchSettings(new BoTorch());
        }

        private void QmcDefaultButton_Click(object sender, EventArgs e)
        {
            TLog.MethodStart();
            SetQMCSettings(new QuasiMonteCarlo());
        }

        private void InitializeSamplerSettings()
        {
            TLog.MethodStart();
            SamplerSettings sampler = _settings.Optimize.Sampler;
            SetTpeSettings(sampler.Tpe);
            SetBoTorchSettings(sampler.BoTorch);
            SetNsga2Settings(sampler.NsgaII);
            SetNsga3Settings(sampler.NsgaIII);
            SetCmaEsSettings(sampler.CmaEs);
            SetQMCSettings(sampler.QMC);
        }

        private void SetTpeSettings(Tpe tpe)
        {
            TLog.MethodStart();
            tpeStartupNumUpDown.Value = tpe.NStartupTrials;
            tpeEINumUpDown.Value = tpe.NEICandidates;
            tpePriorNumUpDown.Value = (decimal)tpe.PriorWeight;

            tpeConsiderPriorCheckBox.Checked = tpe.ConsiderPrior;
            tpeMultivariateCheckBox.Checked = tpe.Multivariate;
            tpeConsiderEndpointsCheckBox.Checked = tpe.ConsiderEndpoints;
            tpeGroupCheckBox.Checked = tpe.Group;
            tpeConsiderMagicClipCheckBox.Checked = tpe.ConsiderMagicClip;
            tpeConstantLiarCheckBox.Checked = tpe.ConstantLiar;
            tpeWarnIndependentSamplingCheckBox.Checked = tpe.WarnIndependentSampling;
        }

        private void SetBoTorchSettings(BoTorch boTorch)
        {
            TLog.MethodStart();
            boTorchStartupNumUpDown.Value = boTorch.NStartupTrials;
        }

        private void SetNsga2Settings(NSGAII nsga)
        {
            TLog.MethodStart();
            nsga2MutationProbCheckBox.Checked = nsga.MutationProb != null;
            nsga2MutationProbUpDown.Enabled = nsga2MutationProbCheckBox.Checked;
            nsga2MutationProbUpDown.Value = nsga.MutationProb != null
                ? (decimal)nsga.MutationProb : 0;
            nsga2CrossoverProbUpDown.Value = (decimal)nsga.CrossoverProb;
            nsga2SwappingProbUpDown.Value = (decimal)nsga.SwappingProb;
            nsga2PopulationSizeUpDown.Value = nsga.PopulationSize;
            nsga2CrossoverCheckBox.Checked = nsga.Crossover != string.Empty;
            nsga2CrossoverComboBox.Enabled = nsga.Crossover != string.Empty;
            nsga2CrossoverComboBox.SelectedIndex = SetNsgaCrossoverSetting(nsga.Crossover);
        }

        private void SetNsga3Settings(NSGAIII nsga)
        {
            TLog.MethodStart();
            nsga3MutationProbCheckBox.Checked = nsga.MutationProb != null;
            nsga3MutationProbUpDown.Enabled = nsga3MutationProbCheckBox.Checked;
            nsga3MutationProbUpDown.Value = nsga.MutationProb != null
                ? (decimal)nsga.MutationProb : 0;
            nsga3CrossoverProbUpDown.Value = (decimal)nsga.CrossoverProb;
            nsga3SwappingProbUpDown.Value = (decimal)nsga.SwappingProb;
            nsga3PopulationSizeUpDown.Value = nsga.PopulationSize;
            nsga3CrossoverCheckBox.Checked = nsga.Crossover != string.Empty;
            nsga3CrossoverComboBox.Enabled = nsga.Crossover != string.Empty;
            nsga3CrossoverComboBox.SelectedIndex = SetNsgaCrossoverSetting(nsga.Crossover);
        }

        private static int SetNsgaCrossoverSetting(string crossover)
        {
            TLog.MethodStart();
            switch (crossover)
            {
                case "":
                case "Uniform":
                    return 0;
                case "BLXAlpha":
                    return 1;
                case "SPX":
                    return 2;
                case "SBX":
                    return 3;
                case "VSBX":
                    return 4;
                case "UNDX":
                    return 5;
                default:
                    throw new ArgumentException("Unexpected crossover method.");
            }
        }

        private void SetCmaEsSettings(CmaEs cmaEs)
        {
            TLog.MethodStart();
            cmaEsStartupNumUpDown.Value = cmaEs.NStartupTrials;
            cmaEsSigmaCheckBox.Checked = cmaEs.Sigma0 != null;
            cmaEsSigmaNumUpDown.Value = cmaEs.Sigma0 != null
                ? (decimal)cmaEs.Sigma0 : 0;
            cmaEsSigmaNumUpDown.Enabled = cmaEsSigmaCheckBox.Checked;

            cmaEsWarnIndependentSamplingCheckBox.Checked = cmaEs.WarnIndependentSampling;
            cmaEsConsiderPruneTrialsCheckBox.Checked = cmaEs.ConsiderPrunedTrials;
            cmaEsUseSaparableCmaCheckBox.Checked = cmaEs.UseSeparableCma;

            cmaEsRestartCheckBox.Checked = cmaEs.RestartStrategy != string.Empty;
            cmaEsIncPopSizeUpDown.Enabled = cmaEsRestartCheckBox.Checked;
            cmaEsIncPopSizeUpDown.Value = cmaEs.IncPopsize;
            cmaEsPopulationSizeUpDown.Enabled = cmaEsRestartCheckBox.Checked;
            cmaEsPopulationSizeUpDown.Value = cmaEs.PopulationSize != null
                ? (decimal)cmaEs.PopulationSize : 1;
        }

        private void SetQMCSettings(QuasiMonteCarlo qmc)
        {
            TLog.MethodStart();
            qmcTypeComboBox.SelectedIndex = qmc.QmcType == "sobol" ? 0 : 1;
            qmcScrambleCheckBox.Checked = qmc.Scramble;
            qmcWarnIndependentSamplingCheckBox.Checked = qmc.WarnIndependentSampling;
            qmcWarnAsyncSeedingCheckBox.Checked = qmc.WarnAsynchronousSeeding;
        }

        private SamplerSettings GetSamplerSettings(SamplerSettings sampler)
        {
            TLog.MethodStart();
            sampler.Tpe = GetTpeSettings(sampler.Tpe);
            sampler.BoTorch = GetBoTorchSettings(sampler.BoTorch);
            sampler.NsgaII = GetNsga2Settings(sampler.NsgaII);
            sampler.NsgaIII = GetNsga3Settings(sampler.NsgaIII);
            sampler.CmaEs = GetCmaEsSettings(sampler.CmaEs);
            sampler.QMC = GetQMCSettings(sampler.QMC);
            return sampler;
        }

        private Tpe GetTpeSettings(Tpe tpe)
        {
            TLog.MethodStart();
            tpe.NStartupTrials = (int)tpeStartupNumUpDown.Value;
            tpe.NEICandidates = (int)tpeEINumUpDown.Value;
            tpe.PriorWeight = (double)tpePriorNumUpDown.Value;
            tpe.ConsiderPrior = tpeConsiderPriorCheckBox.Checked;
            tpe.Multivariate = tpeMultivariateCheckBox.Checked;
            tpe.ConsiderEndpoints = tpeConsiderEndpointsCheckBox.Checked;
            tpe.Group = tpeGroupCheckBox.Checked;
            tpe.ConsiderMagicClip = tpeConsiderMagicClipCheckBox.Checked;
            tpe.ConstantLiar = tpeConstantLiarCheckBox.Checked;
            tpe.WarnIndependentSampling = tpeWarnIndependentSamplingCheckBox.Checked;
            return tpe;
        }

        private BoTorch GetBoTorchSettings(BoTorch boTorch)
        {
            TLog.MethodStart();
            boTorch.NStartupTrials = (int)boTorchStartupNumUpDown.Value;
            return boTorch;
        }

        private NSGAII GetNsga2Settings(NSGAII nsgaII)
        {
            TLog.MethodStart();
            nsgaII.MutationProb = nsga2MutationProbCheckBox.Checked
                ? (double?)nsga2MutationProbUpDown.Value : null;
            nsgaII.CrossoverProb = (double)nsga2CrossoverProbUpDown.Value;
            nsgaII.SwappingProb = (double)nsga2SwappingProbUpDown.Value;
            nsgaII.PopulationSize = (int)nsga2PopulationSizeUpDown.Value;
            nsgaII.Crossover = nsga2CrossoverCheckBox.Checked
                ? nsga2CrossoverComboBox.Text : string.Empty;
            return nsgaII;
        }

        private NSGAIII GetNsga3Settings(NSGAIII nsgaIII)
        {
            TLog.MethodStart();
            nsgaIII.MutationProb = nsga3MutationProbCheckBox.Checked
                ? (double?)nsga3MutationProbUpDown.Value : null;
            nsgaIII.CrossoverProb = (double)nsga3CrossoverProbUpDown.Value;
            nsgaIII.SwappingProb = (double)nsga3SwappingProbUpDown.Value;
            nsgaIII.PopulationSize = (int)nsga3PopulationSizeUpDown.Value;
            nsgaIII.Crossover = nsga3CrossoverCheckBox.Checked
                ? nsga3CrossoverComboBox.Text : string.Empty;
            return nsgaIII;
        }

        private CmaEs GetCmaEsSettings(CmaEs cmaEs)
        {
            TLog.MethodStart();
            cmaEs.NStartupTrials = (int)cmaEsStartupNumUpDown.Value;
            cmaEs.Sigma0 = cmaEsSigmaCheckBox.Checked
                ? (double?)cmaEsSigmaNumUpDown.Value : null;
            cmaEs.WarnIndependentSampling = cmaEsWarnIndependentSamplingCheckBox.Checked;
            cmaEs.ConsiderPrunedTrials = cmaEsConsiderPruneTrialsCheckBox.Checked;
            cmaEs.UseSeparableCma = cmaEsUseSaparableCmaCheckBox.Checked;
            cmaEs.RestartStrategy = cmaEsRestartCheckBox.Checked
                ? "ipop" : string.Empty;
            cmaEs.IncPopsize = (int)cmaEsIncPopSizeUpDown.Value;
            cmaEs.PopulationSize = cmaEsRestartCheckBox.Checked
                ? (int?)cmaEsPopulationSizeUpDown.Value : null;
            cmaEs.UseWarmStart = cmaEsWarmStartCmaEsCheckBox.Checked;
            cmaEs.WarmStartStudyName = cmaEsWarmStartComboBox.Text;
            cmaEs.WithMargin = cmaEsWithMarginCheckBox.Checked;
            return cmaEs;
        }

        private QuasiMonteCarlo GetQMCSettings(QuasiMonteCarlo qmc)
        {
            TLog.MethodStart();
            qmc.QmcType = qmcTypeComboBox.SelectedIndex == 0 ? "sobol" : "halton";
            qmc.Scramble = qmcScrambleCheckBox.Checked;
            qmc.WarnIndependentSampling = qmcWarnIndependentSamplingCheckBox.Checked;
            qmc.WarnAsynchronousSeeding = qmcWarnAsyncSeedingCheckBox.Checked;
            return qmc;
        }

        private void RunGarbageCollectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            _settings.Optimize.GcAfterTrial = (GcAfterTrial)runGarbageCollectionComboBox.SelectedIndex;
        }

        private void MiscLogComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TLog.MethodStart();
            var level = (LogEventLevel)miscLogComboBox.SelectedIndex;
            TLog.SetLoggingLevel(level);
            _settings.LogLevel = level;
        }
    }
}
