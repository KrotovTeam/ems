namespace Common.Constants
{
    public class FilenamesConstants
    {
        #region ClustersJsonFilenames

        public const string B4B5Clusters = @"\B4_B5_clusters.json";

        #endregion

        #region Paths to dynamic result filenames

        public const string PathToCloudMaskTiffFile = @"\cloudMask.tif";
        public const string PathToCloudMaskPngFile = @"\cloudMask.png";
        public const string PathToDynamicFile = @"\dynamic.png";
        public const string PathToEdgedDynamicFile = @"\edged_dynamic.png";
        public const string PathToVisibleImage = @"\visible.png";
        public const string PathToVisibleDynamicFile = @"\visible_dynamic.png";
        public const string PathToDynamicGeoPointsJson = @"\dynamicGeoPoints.json";

        #endregion

        #region Data folders names

        public const string PathToClustersFolder = @"\clusters";
        public const string PathToNormalizedDataFolder = @"\NormalizationData";

        #endregion

        #region Paths to characteristics result

        public const string PathToDamagedAreaResult = @"\areaOfDamage.json";

        #endregion
    }
}
