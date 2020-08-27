using System;

namespace Agibank.Background.Reports.AnalysisSellers
{
    public class AnalysisSellersConfiguration
    {
        private const string MESSAGE = "Parametro {0} no arquivo de configuração não identificado";

        public string FileExtension { get; set; }
        public string PathIn { get; set; }
        public string PathOut { get; set; }
        public string OutputFilename { get; set; }
        public bool WarningAlreadyProcessedFiles { get; set; }
        public bool ReProcessFile { get; set; }


        internal bool IsValid()
        {
            if (string.IsNullOrEmpty(FileExtension))
            {
                throw new ArgumentException(string.Format(MESSAGE, nameof(FileExtension)), nameof(FileExtension));
            }

            if (string.IsNullOrEmpty(PathIn))
            {
                throw new ArgumentException(string.Format(MESSAGE, nameof(PathIn)), nameof(PathIn));
            }

            if (string.IsNullOrEmpty(PathOut))
            {
                throw new ArgumentException(string.Format(MESSAGE, nameof(PathOut)), nameof(PathOut));
            }

            if (string.IsNullOrEmpty(OutputFilename))
            {
                throw new ArgumentException(string.Format(MESSAGE, nameof(OutputFilename)), nameof(OutputFilename));
            }

            return true;
        }
    }

}
