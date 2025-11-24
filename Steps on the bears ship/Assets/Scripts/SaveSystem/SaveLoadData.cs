using System;

[Serializable]
public class SaveLoadData
{
    public string id { get; set; }
    public object[] values { get; set; }
    public SaveLoadData(string id, object[] values)
    {
        this.id = id;
        this.values = values;
    }
}
