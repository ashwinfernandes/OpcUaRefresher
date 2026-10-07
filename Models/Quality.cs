public enum Quality
{
    Good, // the value is valid, current and comes from a healthy sensor or device
    Bad, // the value is totally invalid, e.g. wire break, communication drop
    Uncertain, // when the accuracy of the value cannot be guaranteed e.g. sensor is out of calibration, value is stale
}