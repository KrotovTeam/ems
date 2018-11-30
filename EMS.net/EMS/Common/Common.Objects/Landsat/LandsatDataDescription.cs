using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Constants;
using Common.Enums;

namespace Common.Objects.Landsat
{
    /// <summary>
    /// Описание данных ландсата
    /// </summary>
    public class LandsatDataDescription
    {
        #region Fields

        /// <summary>
        /// Абсолютный путь к файлу метаданных MLT в json
        /// </summary>
        public string MetadataMtlJson { get; set; }

        /// <summary>
        /// Абсолютный путь к файлу метаданных ANG в json
        /// </summary>
        public string MetdataAngJson { get; set; }

        /// <summary>
        /// Канал 1
        /// </summary>
        public LandsatSnapshotDescription Channel1 { get; set; }

        /// <summary>
        /// Канал 2
        /// </summary>
        public LandsatSnapshotDescription Channel2 { get; set; }

        /// <summary>
        /// Канал 3
        /// </summary>
        public LandsatSnapshotDescription Channel3 { get; set; }

        /// <summary>
        /// Канал 4
        /// </summary>
        public LandsatSnapshotDescription Channel4 { get; set; }

        /// <summary>
        /// Канал 5
        /// </summary>
        public LandsatSnapshotDescription Channel5 { get; set; }

        /// <summary>
        /// Канал 6
        /// </summary>
        public LandsatSnapshotDescription Channel6 { get; set; }

        /// <summary>
        /// Канал 7
        /// </summary>
        public LandsatSnapshotDescription Channel7 { get; set; }

        /// <summary>
        /// Канал 8
        /// </summary>
        public LandsatSnapshotDescription Channel8 { get; set; }

        /// <summary>
        /// Канал 9
        /// </summary>
        public LandsatSnapshotDescription Channel9 { get; set; }

        /// <summary>
        /// Канал 10
        /// </summary>
        public string Channel10 { get; set; }

        /// <summary>
        /// Канал 11
        /// </summary>
        public string Channel11 { get; set; }

        /// <summary>
        /// Канал с атмосферными явлениями
        /// </summary>
        public string ChannelBqa { get; set; }

        /// <summary>
        /// Кластеры для 4 и 5 канала в json
        /// </summary>
        //TODO Подумать как это сделать
        public List<string> ClustersJson { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="folder">Папка с данными</param>
        /// <param name="withExceptions">Признак для бросания ошибок</param>
        public LandsatDataDescription(string folder, bool withExceptions = true)
        {
            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentException("Параметр folder пустой");
            }
            if (!Directory.Exists(folder))
            {
                throw new ArgumentException("Папка с данными не существует");
            }

            var filenames = Directory.GetFiles(folder);

            MetadataMtlJson = filenames.SingleOrDefault(name => name.EndsWith("MTL.json", StringComparison.InvariantCultureIgnoreCase));
            if (MetadataMtlJson == null && withExceptions)
            {
                throw new FileNotFoundException("Файл метаданных MLT в json не найден");
            }

            //MetdataAngJson = filenames.SingleOrDefault(name => name.EndsWith("ANG.json", StringComparison.InvariantCultureIgnoreCase));
            //if (MetdataAngJson == null && withExceptions)
            //{
            //    throw new FileNotFoundException("Файл метаданных ANG в json не найден");
            //}

            #region Raw snapshots

            Channel1 = new LandsatSnapshotDescription
            {
                Raw = filenames.SingleOrDefault(name => name.EndsWith("B1.TIF", StringComparison.InvariantCultureIgnoreCase))
            };
            if (Channel1.Raw == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B1 не найден");
            }

            Channel2 = new LandsatSnapshotDescription
            {
                Raw = filenames.SingleOrDefault(name => name.EndsWith("B2.TIF", StringComparison.InvariantCultureIgnoreCase))
            };
            if (Channel2.Raw == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B2 не найден");
            }

            Channel3 = new LandsatSnapshotDescription
            {
                Raw = filenames.SingleOrDefault(name => name.EndsWith("B3.TIF", StringComparison.InvariantCultureIgnoreCase))
            };
            if (Channel3.Raw == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B3 не найден");
            }

            Channel4 = new LandsatSnapshotDescription
            {
                Raw = filenames.SingleOrDefault(name => name.EndsWith("B4.TIF", StringComparison.InvariantCultureIgnoreCase))
            };
            if (Channel4.Raw == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B4 не найден");
            }

            Channel5 = new LandsatSnapshotDescription
            {
                Raw = filenames.SingleOrDefault(name => name.EndsWith("B5.TIF", StringComparison.InvariantCultureIgnoreCase))
            };
            if (Channel5.Raw == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B5 не найден");
            }

            Channel6 = new LandsatSnapshotDescription
            {
                Raw = filenames.SingleOrDefault(name => name.EndsWith("B6.TIF", StringComparison.InvariantCultureIgnoreCase))
            };
            if (Channel6.Raw == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B6 не найден");
            }

            Channel7 = new LandsatSnapshotDescription
            {
                Raw = filenames.SingleOrDefault(name => name.EndsWith("B7.TIF", StringComparison.InvariantCultureIgnoreCase))
            };
            if (Channel7.Raw == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B7 не найден");
            }

            Channel8 = new LandsatSnapshotDescription
            {
                Raw = filenames.SingleOrDefault(name => name.EndsWith("B8.TIF", StringComparison.InvariantCultureIgnoreCase))
            };
            if (Channel8.Raw == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B8 не найден");
            }

            Channel9 = new LandsatSnapshotDescription
            {
                Raw = filenames.SingleOrDefault(name => name.EndsWith("B9.TIF", StringComparison.InvariantCultureIgnoreCase))
            };
            if (Channel9.Raw == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B9 не найден");
            }

            Channel10 = filenames.SingleOrDefault(name => name.EndsWith("B10.TIF", StringComparison.InvariantCultureIgnoreCase));
            if (Channel10 == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B10 не найден");
            }

            Channel11 = filenames.SingleOrDefault(name => name.EndsWith("B11.TIF", StringComparison.InvariantCultureIgnoreCase));
            if (Channel11 == null && withExceptions)
            {
                throw new FileNotFoundException("Сырой снимок B11 не найден");
            }

            ChannelBqa = filenames.SingleOrDefault(name => name.EndsWith("BQA.TIF", StringComparison.InvariantCultureIgnoreCase));
            if (ChannelBqa == null)
            {
                throw new FileNotFoundException("Cнимок BQA не найден");
            }

            #endregion

            #region Normalized snapshots

            var normalizationDataFolder = $@"{folder}{FilenamesConstants.PathToNormalizedDataFolder}";
            if (Directory.Exists(normalizationDataFolder))
            {
                var normalizedFilenames = Directory.GetFiles(normalizationDataFolder);

                Channel1.Normalized = normalizedFilenames.SingleOrDefault(name =>
                    name.EndsWith("B1.TIF.l8n", StringComparison.InvariantCultureIgnoreCase));
                Channel2.Normalized = normalizedFilenames.SingleOrDefault(name =>
                    name.EndsWith("B2.TIF.l8n", StringComparison.InvariantCultureIgnoreCase));
                Channel3.Normalized = normalizedFilenames.SingleOrDefault(name =>
                    name.EndsWith("B3.TIF.l8n", StringComparison.InvariantCultureIgnoreCase));
                Channel4.Normalized = normalizedFilenames.SingleOrDefault(name =>
                    name.EndsWith("B4.TIF.l8n", StringComparison.InvariantCultureIgnoreCase));
                Channel5.Normalized = normalizedFilenames.SingleOrDefault(name =>
                    name.EndsWith("B5.TIF.l8n", StringComparison.InvariantCultureIgnoreCase));
                Channel6.Normalized = normalizedFilenames.SingleOrDefault(name =>
                    name.EndsWith("B6.TIF.l8n", StringComparison.InvariantCultureIgnoreCase));
                Channel7.Normalized = normalizedFilenames.SingleOrDefault(name =>
                    name.EndsWith("B7.TIF.l8n", StringComparison.InvariantCultureIgnoreCase));
                Channel8.Normalized = normalizedFilenames.SingleOrDefault(name =>
                    name.EndsWith("B8.TIF.l8n", StringComparison.InvariantCultureIgnoreCase));
                Channel9.Normalized = normalizedFilenames.SingleOrDefault(name =>
                    name.EndsWith("B9.TIF.l8n", StringComparison.InvariantCultureIgnoreCase));
                // у 10 и 11 канала отсутствуют нормализованные файлы
            }

            ClustersJson = new List<string>();

            var clustersFolder = $@"{folder}{FilenamesConstants.PathToClustersFolder}";

            if (Directory.Exists(clustersFolder))
            {
                var clustersFiles = Directory.GetFiles(clustersFolder);
                if (clustersFiles.Any())
                {
                   ClustersJson.AddRange(clustersFiles);
                } 
            }
            else
            {
                Directory.CreateDirectory(clustersFolder);
            }

            #endregion

        }

        public LandsatSnapshotDescription GetLandsatSnapshotDescriptionByChannel(Landsat8Channel channel)
        {
            switch (channel)
            {
                case Landsat8Channel.Channel4:
                    return Channel4;
                case Landsat8Channel.Channel5:
                    return Channel5;
                default:
                    return null;
            }
        }

        #endregion
    }
}
