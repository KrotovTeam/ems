using System;

namespace Common.Objects
{
    public class MetadataFileInfo
    {
        public string Origin { get; set; }
        public string RequestId { get; set; }
        public string LandsatSceneId { get; set; }
        public string LandsatProductId { get; set; }
        public string CollectionNumber { get; set; }
        public DateTime FileDate { get; set; }
        public string StationId { get; set; }
        public string ProcessingSoftwareVersion { get; set; }
    }

    public class ProductMetadata
    {
        public string DataType { get; set; }
        public string CollectionCategory { get; set; }
        public string ElevationSource { get; set; }
        public string OutputFormat { get; set; }
        public string SpacecraftId { get; set; }
        public string SensorId { get; set; }
        public string WrsPath { get; set; }
        public string WrsRow { get; set; }
        public string NadirOffnadir { get; set; }
        public string TargetWrsPath { get; set; }
        public string TargetWrsRow { get; set; }
        public string DateAcquired { get; set; }
        public string SceneCenterTime { get; set; }
        public string CornerUlLatProduct { get; set; }
        public string CornerUlLonProduct { get; set; }
        public string CornerUrLatProduct { get; set; }
        public string CornerUrLonProduct { get; set; }
        public string CornerLlLatProduct { get; set; }
        public string CornerLlLonProduct { get; set; }
        public string CornerLrLatProduct { get; set; }
        public string CornerLrLonProduct { get; set; }
        public string CornerUlProjectionXProduct { get; set; }
        public string CornerUlProjectionYProduct { get; set; }
        public string CornerUrProjectionXProduct { get; set; }
        public string CornerUrProjectionYProduct { get; set; }
        public string CornerLlProjectionXProduct { get; set; }
        public string CornerLlProjectionYProduct { get; set; }
        public string CornerLrProjectionXProduct { get; set; }
        public string CornerLrProjectionYProduct { get; set; }
        public string PanchromaticLines { get; set; }
        public string PanchromaticSamples { get; set; }
        public string ReflectiveLines { get; set; }
        public string ReflectiveSamples { get; set; }
        public string ThermalLines { get; set; }
        public string ThermalSamples { get; set; }
        public string FileNameBand1 { get; set; }
        public string FileNameBand2 { get; set; }
        public string FileNameBand3 { get; set; }
        public string FileNameBand4 { get; set; }
        public string FileNameBand5 { get; set; }
        public string FileNameBand6 { get; set; }
        public string FileNameBand7 { get; set; }
        public string FileNameBand8 { get; set; }
        public string FileNameBand9 { get; set; }
        public string FileNameBand10 { get; set; }
        public string FileNameBand11 { get; set; }
        public string FileNameBandQuality { get; set; }
        public string AngleCoefficientFileName { get; set; }
        public string MetadataFileName { get; set; }
        public string CpfName { get; set; }
        public string BpfNameOli { get; set; }
        public string BpfNameTirs { get; set; }
        public string RlutFileName { get; set; }
    }

    public class ImageAttributes
    {
        public string CloudCover { get; set; }
        public string CloudCoverLand { get; set; }
        public string ImageQualityOli { get; set; }
        public string ImageQualityTirs { get; set; }
        public string TirsSsmModel { get; set; }
        public string TirsSsmPositionStatus { get; set; }
        public string TirsStrayLightCorrectionSource { get; set; }
        public string RollAngle { get; set; }
        public string SunAzimuth { get; set; }
        public double SunElevation { get; set; }
        public double EarthSunDistance { get; set; }
        public string SaturationBand1 { get; set; }
        public string SaturationBand2 { get; set; }
        public string SaturationBand3 { get; set; }
        public string SaturationBand4 { get; set; }
        public string SaturationBand5 { get; set; }
        public string SaturationBand6 { get; set; }
        public string SaturationBand7 { get; set; }
        public string SaturationBand8 { get; set; }
        public string SaturationBand9 { get; set; }
        public string GroundControlPointsVersion { get; set; }
        public string GroundControlPointsModel { get; set; }
        public string GeometricRmseModel { get; set; }
        public string GeometricRmseModelY { get; set; }
        public string GeometricRmseModelX { get; set; }
        public string GroundControlPointsVerify { get; set; }
        public string GeometricRmseVerify { get; set; }
        public string TruncationOli { get; set; }
    }

    public class MinMaxRadiance
    {
        public double RadianceMaximumBand1 { get; set; }
        public double RadianceMinimumBand1 { get; set; }
        public double RadianceMaximumBand2 { get; set; }
        public double RadianceMinimumBand2 { get; set; }
        public double RadianceMaximumBand3 { get; set; }
        public double RadianceMinimumBand3 { get; set; }
        public double RadianceMaximumBand4 { get; set; }
        public double RadianceMinimumBand4 { get; set; }
        public double RadianceMaximumBand5 { get; set; }
        public double RadianceMinimumBand5 { get; set; }
        public double RadianceMaximumBand6 { get; set; }
        public double RadianceMinimumBand6 { get; set; }
        public double RadianceMaximumBand7 { get; set; }
        public double RadianceMinimumBand7 { get; set; }
        public double RadianceMaximumBand8 { get; set; }
        public double RadianceMinimumBand8 { get; set; }
        public double RadianceMaximumBand9 { get; set; }
        public double RadianceMinimumBand9 { get; set; }
        public double RadianceMaximumBand10 { get; set; }
        public double RadianceMinimumBand10 { get; set; }
        public double RadianceMaximumBand11 { get; set; }
        public double RadianceMinimumBand11 { get; set; }
    }

    public class MinMaxReflectance
    {
        public double ReflectanceMaximumBand1 { get; set; }
        public double ReflectanceMinimumBand1 { get; set; }
        public double ReflectanceMaximumBand2 { get; set; }
        public double ReflectanceMinimumBand2 { get; set; }
        public double ReflectanceMaximumBand3 { get; set; }
        public double ReflectanceMinimumBand3 { get; set; }
        public double ReflectanceMaximumBand4 { get; set; }
        public double ReflectanceMinimumBand4 { get; set; }
        public double ReflectanceMaximumBand5 { get; set; }
        public double ReflectanceMinimumBand5 { get; set; }
        public double ReflectanceMaximumBand6 { get; set; }
        public double ReflectanceMinimumBand6 { get; set; }
        public double ReflectanceMaximumBand7 { get; set; }
        public double ReflectanceMinimumBand7 { get; set; }
        public double ReflectanceMaximumBand8 { get; set; }
        public double ReflectanceMinimumBand8 { get; set; }
        public double ReflectanceMaximumBand9 { get; set; }
        public double ReflectanceMinimumBand9 { get; set; }
    }

    public class MinMaxPixelValue
    {
        public string QuantizeCalMaxBand1 { get; set; }
        public string QuantizeCalMinBand1 { get; set; }
        public string QuantizeCalMaxBand2 { get; set; }
        public string QuantizeCalMinBand2 { get; set; }
        public string QuantizeCalMaxBand3 { get; set; }
        public string QuantizeCalMinBand3 { get; set; }
        public string QuantizeCalMaxBand4 { get; set; }
        public string QuantizeCalMinBand4 { get; set; }
        public string QuantizeCalMaxBand5 { get; set; }
        public string QuantizeCalMinBand5 { get; set; }
        public string QuantizeCalMaxBand6 { get; set; }
        public string QuantizeCalMinBand6 { get; set; }
        public string QuantizeCalMaxBand7 { get; set; }
        public string QuantizeCalMinBand7 { get; set; }
        public string QuantizeCalMaxBand8 { get; set; }
        public string QuantizeCalMinBand8 { get; set; }
        public string QuantizeCalMaxBand9 { get; set; }
        public string QuantizeCalMinBand9 { get; set; }
        public string QuantizeCalMaxBand10 { get; set; }
        public string QuantizeCalMinBand10 { get; set; }
        public string QuantizeCalMaxBand11 { get; set; }
        public string QuantizeCalMinBand11 { get; set; }
    }

    public class RadiometricRescaling
    {
        public double RadianceMultBand1 { get; set; }
        public double RadianceMultBand2 { get; set; }
        public double RadianceMultBand3 { get; set; }
        public double RadianceMultBand4 { get; set; }
        public double RadianceMultBand5 { get; set; }
        public double RadianceMultBand6 { get; set; }
        public double RadianceMultBand7 { get; set; }
        public double RadianceMultBand8 { get; set; }
        public double RadianceMultBand9 { get; set; }
        public double RadianceMultBand10 { get; set; }
        public double RadianceMultBand11 { get; set; }
        public double RadianceAddBand1 { get; set; }
        public double RadianceAddBand2 { get; set; }
        public double RadianceAddBand3 { get; set; }
        public double RadianceAddBand4 { get; set; }
        public double RadianceAddBand5 { get; set; }
        public double RadianceAddBand6 { get; set; }
        public double RadianceAddBand7 { get; set; }
        public double RadianceAddBand8 { get; set; }
        public double RadianceAddBand9 { get; set; }
        public double RadianceAddBand10 { get; set; }
        public double RadianceAddBand11 { get; set; }
        public double ReflectanceMultBand1 { get; set; }
        public double ReflectanceMultBand2 { get; set; }
        public double ReflectanceMultBand3 { get; set; }
        public double ReflectanceMultBand4 { get; set; }
        public double ReflectanceMultBand5 { get; set; }
        public double ReflectanceMultBand6 { get; set; }
        public double ReflectanceMultBand7 { get; set; }
        public double ReflectanceMultBand8 { get; set; }
        public double ReflectanceMultBand9 { get; set; }
        public double ReflectanceAddBand1 { get; set; }
        public double ReflectanceAddBand2 { get; set; }
        public double ReflectanceAddBand3 { get; set; }
        public double ReflectanceAddBand4 { get; set; }
        public double ReflectanceAddBand5 { get; set; }
        public double ReflectanceAddBand6 { get; set; }
        public double ReflectanceAddBand7 { get; set; }
        public double ReflectanceAddBand8 { get; set; }
        public double ReflectanceAddBand9 { get; set; }
    }

    public class TirsThermalConstants
    {
        public string K1ConstantBand10 { get; set; }
        public string K2ConstantBand10 { get; set; }
        public string K1ConstantBand11 { get; set; }
        public string K2ConstantBand11 { get; set; }
    }

    public class ProjectionParameters
    {
        public string MapProjection { get; set; }
        public string Datum { get; set; }
        public string Ellipsoid { get; set; }
        public string UtmZone { get; set; }
        public string GridCellSizePanchromatic { get; set; }
        public string GridCellSizeReflective { get; set; }
        public string GridCellSizeThermal { get; set; }
        public string Orientation { get; set; }
        public string ResamplingOption { get; set; }
    }

    public class L1MetadataFile
    {
        public MetadataFileInfo MetadataFileInfo { get; set; }
        public ProductMetadata ProductMetadata { get; set; }
        public ImageAttributes ImageAttributes { get; set; }
        public MinMaxRadiance MinMaxRadiance { get; set; }
        public MinMaxReflectance MinMaxReflectance { get; set; }
        public MinMaxPixelValue MinMaxPixelValue { get; set; }
        public RadiometricRescaling RadiometricRescaling { get; set; }
        public TirsThermalConstants TirsThermalConstants { get; set; }
        public ProjectionParameters ProjectionParameters { get; set; }
    }

    public class LandsatMetadata
    {
        public L1MetadataFile L1MetadataFile { get; set; }
    }
}
