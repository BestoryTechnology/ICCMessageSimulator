
using System.Collections ;
using System.Collections.Generic ;
using System.IO ;
using System ;
using System.Text ;

public class FSIOHelper 
{

    private static string currentDirectory = string.Empty;

    public static string GetCurrentDirectory()
    {
        if (string.IsNullOrEmpty(currentDirectory))
        {
#if UNITY_EDITOR
            currentDirectory = Environment.CurrentDirectory;
#elif UNITY_STANDALONE
			string fullFileName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase ;
			int lastIndex = fullFileName.LastIndexOf("/");
			currentDirectory = fullFileName.Substring(8, lastIndex - 8);
			
			DirectoryInfo dir = Directory.GetParent(currentDirectory).Parent;
			currentDirectory = dir.FullName;
#else
			string fullFileName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase ;
            int lastIndex = fullFileName.LastIndexOf("/");
            currentDirectory = fullFileName.Substring(8, lastIndex - 8); 
#endif
        }

        return currentDirectory;
    }

}
