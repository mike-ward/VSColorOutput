using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;

namespace VSColorOutput.Output.GCCErrorList
{
    public static class GCCErrorGenerator
    {
        private static ErrorListProvider _errorListProvider;

        private static readonly HashSet<GCCErrorListItem> _currentListItems = new HashSet<GCCErrorListItem>();

        private static readonly IVsUIShellOpenDocument _vsUiShellOpenDocument = Package.GetGlobalService(typeof(IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _errorListProvider = new ErrorListProvider(serviceProvider);
        }

        public static void AddError(GCCErrorListItem item)
        {
            AddTask(item, TaskErrorCategory.Error);
        }

        public static void AddWarning(GCCErrorListItem item)
        {
            AddTask(item, TaskErrorCategory.Warning);
        }

        public static void AddMessage(GCCErrorListItem item)
        {
            AddTask(item, TaskErrorCategory.Message);
        }

        public static void DismissAllTasks()
        {
            _errorListProvider.Tasks.Clear();
            _currentListItems.Clear();
        }

        private static void AddTask(GCCErrorListItem item, TaskErrorCategory category)
        {
            // Visual studio has a bug of showing each gcc message twice. Make sure this error list item doesn't exist already
            if (item == null || _currentListItems.Contains(item))
            {
                return;
            }

            var task = new ErrorTask
            {
                Category = TaskCategory.BuildCompile,
                ErrorCategory = category,
                Text = item.Text
            };

            switch (item.ErrorType)
            {
                case GCCErrorType.Full:
                    task.Navigate += TaskOnNavigate;
                    task.Line = item.Line - 1; // Visual studio starts counting from 0
                    task.Column = item.Column - 1; // Visual studio starts counting from 0
                    task.Document = GetFileByProjectNumber(item.ProjectNumber, item.Filename);
                    task.HierarchyItem = GetItemHierarchy(task.Document);
                    break;
                case GCCErrorType.GCCOnly:
                    task.Document = item.Filename;
                    var project = GetProjectByNumber(item.ProjectNumber);
                    task.HierarchyItem = GetProjectHierarchy(project);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _errorListProvider.Tasks.Add(task);
            _currentListItems.Add(item);
        }

        private static void TaskOnNavigate(object sender, EventArgs eventArgs)
        {
            Task task = sender as Task;
            if (task == null)
            {
                throw new ArgumentException("sender");
            }
            task.Line++; // Navigation starts counting from 1, do ++
            _errorListProvider.Navigate(task, new Guid(EnvDTE.Constants.vsViewKindCode));
            task.Line--; // Back to normal, do --
        }

        private static IVsUIHierarchy GetItemHierarchy(string filename)
        {
            if (_vsUiShellOpenDocument != null)
            {
                Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider;
                IVsWindowFrame frame;
                IVsUIHierarchy hierarchy;
                uint itemId;
                Guid logicalView = VSConstants.LOGVIEWID_Code;

                if (ErrorHandler.Succeeded(_vsUiShellOpenDocument.OpenDocumentViaProject(filename, ref logicalView, out serviceProvider, out hierarchy, out itemId, out frame)))
                {
                    return hierarchy;
                }
            }

            return null;
        }

        public static IVsHierarchy GetProjectHierarchy(Project project)
        {
            IVsHierarchy hierarchy;

            // Get the vs solution
            IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(IVsSolution));
            int hr = solution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);

            if (hr == VSConstants.S_OK)
            {
                return hierarchy;
            }

            return null;
        }


        private static Project GetProjectByNumber(int number)
        {
            var dte = (EnvDTE80.DTE2)Package.GetGlobalService(typeof(EnvDTE.DTE));
            return dte.Solution.Projects.Item(number);
        }

        private static string GetFileByProjectNumber(int number, string filename)
        {
            var proj = GetProjectByNumber(number);
            
            string projectPath = Path.GetDirectoryName(proj.FileName);

            foreach (string file in Directory.EnumerateFiles(projectPath, "*.*", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file) == filename)
                {
                    return file;
                }
            }
            
            return filename;
        }
    }
}
