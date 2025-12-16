using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class LogEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("SID")]
    public long SequenceId { get; set; }

    [BsonElement("TITLE")]
    public string Title { get; set; }

    [BsonElement("MESSAGE")]
    public string Message { get; set; }

    [BsonElement("START")]
    public DateTime StartTime { get; set; }

    [BsonElement("END")]
    public DateTime EndTime { get; set; }

    [BsonElement("TIMESPAN")]
    public double TimeSpan { get; set; }

    [BsonElement("CATEGORY")]
    public string Category { get; set; }

    [BsonElement("TAGS")]
    public string Tags { get; set; }

    [BsonElement("DATA")]
    public string Data { get; set; }

    [BsonElement("USER")]
    public string User { get; set; }

    [BsonElement("EXCEPTION")]
    public string Exception { get; set; }

    [BsonElement("LEVEL")]
    public string Level { get; set; }

    [BsonElement("LOGIC_NO")]
    public string LogicNo { get; set; }

    [BsonElement("CALL_SITE")]
    public string CallSite { get; set; }

    [BsonElement("LINE_NO")]
    public int LineNo { get; set; }

    [BsonElement("LOGGER")]
    public string Logger { get; set; }

    [BsonElement("MACHINE_NAME")]
    public string MachineName { get; set; }

    [BsonElement("PROCESS_ID")]
    public int ProcessId { get; set; }

    [BsonElement("PROCESS_NAME")]
    public string ProcessName { get; set; }

    [BsonElement("UTC_TIME")]
    public DateTime UtcTime { get; set; }

    [BsonElement("LOCAL_TIME")]
    public DateTime LocalTime { get; set; }
}