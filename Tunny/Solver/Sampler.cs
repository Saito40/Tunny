using System;

using Python.Runtime;

using Tunny.Settings;
using Tunny.Settings.Sampler;
using Tunny.Util;

namespace Tunny.Solver
{
    public static class Sampler
    {
        internal static dynamic Random(dynamic optuna, TunnySettings settings)
        {
            TLog.MethodStart();
            Settings.Sampler.Random random = settings.Optimize.Sampler.Random;
            return optuna.samplers.RandomSampler(
                seed: random.Seed
            );
        }

        internal static dynamic CmaEs(dynamic optuna, TunnySettings settings)
        {
            TLog.MethodStart();
            CmaEs cmaEs = settings.Optimize.Sampler.CmaEs;
            return cmaEs.UseWarmStart
                ? optuna.samplers.CmaEsSampler(
                    n_startup_trials: cmaEs.NStartupTrials,
                    warn_independent_sampling: cmaEs.WarnIndependentSampling,
                    seed: cmaEs.Seed,
                    consider_pruned_trials: cmaEs.ConsiderPrunedTrials,
                    restart_strategy: cmaEs.RestartStrategy == string.Empty ? null : cmaEs.RestartStrategy,
                    inc_popsize: cmaEs.IncPopsize,
                    popsize: cmaEs.PopulationSize,
                    source_trials: optuna.load_study(study_name: cmaEs.WarmStartStudyName, storage: settings.Storage.GetOptunaStoragePath()).get_trials(),
                    with_margin: cmaEs.WithMargin
                )
                : optuna.samplers.CmaEsSampler(
                    sigma0: cmaEs.Sigma0,
                    n_startup_trials: cmaEs.NStartupTrials,
                    warn_independent_sampling: cmaEs.WarnIndependentSampling,
                    seed: cmaEs.Seed,
                    consider_pruned_trials: cmaEs.ConsiderPrunedTrials,
                    restart_strategy: cmaEs.RestartStrategy == string.Empty ? null : cmaEs.RestartStrategy,
                    inc_popsize: cmaEs.IncPopsize,
                    popsize: cmaEs.PopulationSize,
                    use_separable_cma: cmaEs.UseSeparableCma,
                    with_margin: cmaEs.WithMargin
                );
        }

        internal static dynamic NSGAII(dynamic optuna, TunnySettings settings, bool hasConstraints)
        {
            TLog.MethodStart();
            NSGAII nsga2 = settings.Optimize.Sampler.NsgaII;
            return optuna.samplers.NSGAIISampler(
                population_size: nsga2.PopulationSize,
                mutation_prob: nsga2.MutationProb,
                crossover_prob: nsga2.CrossoverProb,
                swapping_prob: nsga2.SwappingProb,
                seed: nsga2.Seed,
                crossover: SetCrossover(optuna, nsga2.Crossover),
                constraints_func: hasConstraints ? ConstraintFunc() : null
            );
        }

        internal static dynamic NSGAIII(dynamic optuna, TunnySettings settings, bool hasConstraints)
        {
            TLog.MethodStart();
            NSGAIII nsga3 = settings.Optimize.Sampler.NsgaIII;
            return optuna.samplers.NSGAIIISampler(
                population_size: nsga3.PopulationSize,
                mutation_prob: nsga3.MutationProb,
                crossover_prob: nsga3.CrossoverProb,
                swapping_prob: nsga3.SwappingProb,
                seed: nsga3.Seed,
                crossover: SetCrossover(optuna, nsga3.Crossover),
                constraints_func: hasConstraints ? ConstraintFunc() : null
            );
        }

        private static dynamic SetCrossover(dynamic optuna, string crossover)
        {
            TLog.MethodStart();
            switch (crossover)
            {
                case "Uniform":
                    return optuna.samplers.nsgaii.UniformCrossover();
                case "BLXAlpha":
                    return optuna.samplers.nsgaii.BLXAlphaCrossover();
                case "SPX":
                    return optuna.samplers.nsgaii.SPXCrossover();
                case "SBX":
                    return optuna.samplers.nsgaii.SBXCrossover();
                case "VSBX":
                    return optuna.samplers.nsgaii.VSBXCrossover();
                case "UNDX":
                    return optuna.samplers.nsgaii.UNDXCrossover();
                case "":
                    return null;
                default:
                    throw new ArgumentException("Unexpected crossover setting");
            }
        }

        internal static dynamic TPE(dynamic optuna, TunnySettings settings, bool hasConstraints)
        {
            TLog.MethodStart();
            Tpe tpe = settings.Optimize.Sampler.Tpe;
            return optuna.samplers.TPESampler(
                seed: tpe.Seed,
                consider_prior: tpe.ConsiderPrior,
                prior_weight: 1.0,
                consider_magic_clip: tpe.ConsiderMagicClip,
                consider_endpoints: tpe.ConsiderEndpoints,
                n_startup_trials: tpe.NStartupTrials,
                n_ei_candidates: tpe.NEICandidates,
                multivariate: tpe.Multivariate,
                group: tpe.Group,
                warn_independent_sampling: tpe.WarnIndependentSampling,
                constant_liar: tpe.ConstantLiar,
                constraints_func: hasConstraints ? ConstraintFunc() : null
            );
        }

        internal static dynamic BoTorch(dynamic optuna, TunnySettings settings, bool hasConstraints)
        {
            TLog.MethodStart();
            BoTorch boTorch = settings.Optimize.Sampler.BoTorch;
            return optuna.integration.BoTorchSampler(
                seed: boTorch.Seed,
                n_startup_trials: boTorch.NStartupTrials,
                constraints_func: hasConstraints ? ConstraintFunc() : null
            );
        }

        internal static dynamic QMC(dynamic optuna, TunnySettings settings)
        {
            TLog.MethodStart();
            QuasiMonteCarlo qmc = settings.Optimize.Sampler.QMC;
            return optuna.samplers.QMCSampler(
                qmc_type: qmc.QmcType,
                scramble: qmc.Scramble,
                seed: qmc.Seed,
                warn_independent_sampling: qmc.WarnIndependentSampling
            );
        }

        private static dynamic ConstraintFunc()
        {
            TLog.MethodStart();
            PyModule ps = Py.CreateScope();
            ps.Exec(
                "def constraints(trial):\n" +
                "  return trial.user_attrs[\"Constraint\"]\n"
            );
            return ps.Get("constraints");
        }
    }
}
