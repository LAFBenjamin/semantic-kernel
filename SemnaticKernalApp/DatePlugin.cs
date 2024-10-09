using Microsoft.SemanticKernel;
using System.ComponentModel;

public class DatePlugin
{
   [KernelFunction("GetCurrentDate")]
    [Description("Displays the current date.")]
    public static string GetCurrentDate()
        => DateTime.Today.ToLongDateString();

    [KernelFunction("GetTime")]
    [Description("Displays the current time.")]
    public static string GetTime()
        => DateTime.Now.ToShortTimeString();
}