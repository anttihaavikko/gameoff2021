using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

public static class TextFormatter
{
    private const string RedHex = "#f580c6";
    private const string GreenHex = "#56e87f";
    private const string YellowHex = "#e2ea8f";
    private const string BlueHex = "#87d1e2";
    private const string PurpleHex = "#be99f2";
    private const string OrangeHex = "#f3b874";
    private const string CommentHex = "#6d7cad";

    public static Color Red => GetColor(RedHex);
    public static Color Green => GetColor(GreenHex);
    public static Color Yellow => GetColor(YellowHex);
    public static Color Blue => GetColor(BlueHex);
    public static Color Purple => GetColor(PurpleHex);
    public static Color Orange => GetColor(OrangeHex);
    public static Color Comment => GetColor(CommentHex);
    
    public static string DoColors(string content)
    {
        var sb = new StringBuilder(content);

        sb.Replace("<y>", $"<color={YellowHex}>");
        sb.Replace("<r>", $"<color={RedHex}>");
        sb.Replace("<g>", $"<color={GreenHex}>");
        sb.Replace("<b>", $"<color={BlueHex}>");
        sb.Replace("<p>", $"<color={PurpleHex}>");
        sb.Replace("<o>", $"<color={OrangeHex}>");
        sb.Replace("<c>", $"<color={CommentHex}>");
        sb.Replace("</c>", "</color>");
        sb.Replace("</y>", "</color>");
        sb.Replace("</r>", "</color>");
        sb.Replace("</g>", "</color>");
        sb.Replace("</b>", "</color>");
        sb.Replace("</p>", "</color>");
        sb.Replace("</o>", "</color>");
        sb.Replace("\t", "  ");

        return sb.ToString();
    }

    private static Color GetColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var color);
        return color;
    }
}