/***
*	Title："基础工具" 项目
*		主题：路径帮助类
*	Description：
*		功能：
*		    1、【保存】获取到选中的文件路径和文件名称
*		    2、【打开】获取到选中的文件路径和文件名称
*		    3、获取选择的目录
*	Date：2021
*	Version：0.1版本
*	Author：Coffee
*	Modify Recoder：
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utils
{
    public class PathHelper
    {
        const string filterFormat = "ini文件(*.ini)|*.ini|txt(*.txt)|*.txt";

        /// <summary>
        /// 【保存】获取到选中的文件路径和文件名称
        /// </summary>
        /// <param name="titleName">标题名称</param>
        /// <param name="filterFormat">过滤格式（指定可以选择的文件类型）</param>
        /// <returns></returns>
        public static string SaveSelectedFilePathAndName(string titleName = "保存配置文件")
        {
            string filePathAndName = null;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = titleName;
            saveFileDialog.Filter = filterFormat;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePathAndName = saveFileDialog.FileName;
            }
            return filePathAndName;
        }

        /// <summary>
        /// 【打开】获取到选中的文件路径和文件名称
        /// </summary>
        /// <param name="titleName">标题名称</param>
        /// <param name="filterFormat">过滤格式（指定可以选择的文件类型）</param>
        /// <returns></returns>
        public static string OpenSelectedFilePathAndName(string titleName = "打开配置文件")
        {
            string filePathAndName = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = titleName;
            openFileDialog.Filter = filterFormat;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePathAndName = openFileDialog.FileName;
            }
            return filePathAndName;
        }

        /// <summary>
        /// 【打开】获取选择的目录
        /// </summary>
        /// <returns>返回选择的目录</returns>
        public static string GetSelectDirectory()
        {
            string filePath = null;
            FolderBrowserDialog dir = new FolderBrowserDialog();

            if (dir.ShowDialog() == DialogResult.OK)
            {
                filePath = dir.SelectedPath;
            }

            return filePath;
        }

        /// <summary>
        /// 【删除】删除文件夹
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <returns></returns>
        public static string DeleteFile(string fileFullPath)
        {
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
            }
            return "无该文件:" + fileFullPath;
        }
    }
}