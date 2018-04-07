﻿using Resourcer;

namespace Timestamp
{
    using System;
    using System.IO;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class AddTimestamp : Task
    {

        [Required]
        public string SolutionDirectory { get; set; }

        [Required]
        public string ProjectFile { get; set; }

        [Required]
        public ITaskItem[] CompileFiles { get; set; }

        [Output]
        public string TimestampAttributeTempFilePath { get; set; }


        public override bool Execute()
        {
            try
            {
                TempFileTracker.DeleteTempFiles();

                CreateTempAssemblyInfo();

                return true;
            }
            catch (Exception exception)
            {
                BuildEngine.LogErrorEvent(new BuildErrorEventArgs("", "", null, 0, 0, 0, 0, "Error occurred: " + exception, "", "Timestamp"));
                return false;
            }
        }

        void CreateTempAssemblyInfo()
        {
            var timestampAttributeUsage = string.Format(@"
using Timestamp;
[assembly: Timestamp(""{0}"")]

", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));

            var timestampAttribute = Resource.AsString("TimestampAttribute.cs");

            var tempFileContent = timestampAttributeUsage + timestampAttribute;

            var tempFileName = string.Format("Timestamp_{0}_{1}.cs", Path.GetFileNameWithoutExtension(ProjectFile), Path.GetRandomFileName());
            TimestampAttributeTempFilePath = Path.Combine(TempFileTracker.TempPath, tempFileName);
            File.WriteAllText(TimestampAttributeTempFilePath, tempFileContent);
        }


    }
}