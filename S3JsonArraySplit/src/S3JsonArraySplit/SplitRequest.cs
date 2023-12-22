namespace S3JsonArraySplit;

public class SplitRequest
{
    public string Region { get; set; }
    public string Bucketname { get; set; }
    public string Source { get; set; }
    public string DestinationPathTemplate { get; set; }
    public string Fieldname { get; set; }
    public bool AddContext { get; set; }
    public string DefaultCategory { get; set; }
    public string ResultPathPrefix {get; set; }
    public string FilenameSyntax {get; set;}
}