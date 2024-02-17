﻿using System;
using System.IO;

using Python.Runtime;

using Tunny.Core.Enum;
using Tunny.Storage;
using Tunny.Util;

namespace Tunny.Settings
{
    public class Storage
    {
        public string Path { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/fish.log";
        public StorageType Type { get; set; } = StorageType.Journal;

        private string GetArtifactBackendPath()
        {
            TLog.MethodStart();
            return System.IO.Path.GetDirectoryName(Path) + "/artifacts";
        }

        public string GetOptunaStoragePath()
        {
            TLog.MethodStart();
            switch (Type)
            {
                case StorageType.InMemory:
                    return string.Empty;
                case StorageType.Sqlite:
                    return "sqlite:///" + Path;
                case StorageType.Postgres:
                case StorageType.MySql:
                    throw new NotImplementedException();
                case StorageType.Journal:
                    return Path;
                default:
                    throw new NotImplementedException();
            }
        }

        public string GetOptunaStoragePathByExtension()
        {
            TLog.MethodStart();
            switch (System.IO.Path.GetExtension(Path))
            {
                case null:
                    return string.Empty;
                case ".sqlite3":
                case ".db":
                    return "sqlite:///" + Path;
                case ".log":
                    return Path;
                default:
                    throw new NotImplementedException();
            }
        }

        public string GetOptunaStorageCommandLinePathByExtension()
        {
            TLog.MethodStart();
            switch (System.IO.Path.GetExtension(Path))
            {
                case null:
                    return string.Empty;
                case ".sqlite3":
                case ".db":
                    return @"sqlite:///" + $"\"{Path}\"";
                case ".log":
                    return $"\"{Path}\"";
                default:
                    throw new NotImplementedException();
            }
        }

        public dynamic CreateNewOptunaStorage(bool useInnerPythonEngine)
        {
            TLog.MethodStart();
            dynamic storage;
            switch (Type)
            {
                case StorageType.InMemory:
                    storage = new InMemoryStorage().CreateNewStorage(useInnerPythonEngine, this);
                    break;
                case StorageType.Sqlite:
                    storage = new SqliteStorage().CreateNewStorage(useInnerPythonEngine, this);
                    break;
                case StorageType.Journal:
                    storage = new JournalStorage().CreateNewStorage(useInnerPythonEngine, this);
                    break;
                default:
                    throw new ArgumentException("Storage type is not defined.");
            }

            return storage;
        }

        public dynamic CreateNewOptunaArtifactBackend(bool useInnerPythonEngine)
        {
            TLog.MethodStart();
            dynamic backend;

            if (useInnerPythonEngine)
            {
                PythonEngine.Initialize();
                using (Py.GIL())
                {
                    backend = CreateArtifactBackendProcess();
                }
                PythonEngine.Shutdown();
            }
            else
            {
                backend = CreateArtifactBackendProcess();
            }

            return backend;
        }

        private dynamic CreateArtifactBackendProcess()
        {
            TLog.MethodStart();
            dynamic optuna = Py.Import("optuna");
            string backendPath = GetArtifactBackendPath();
            Directory.CreateDirectory(backendPath);
            return optuna.artifacts.FileSystemArtifactStore(base_path: backendPath);
        }
    }
}
