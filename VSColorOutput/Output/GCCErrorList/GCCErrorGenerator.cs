using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSColorOutput.State;
using Constants = EnvDTE.Constants;

namespace VSColorOutput.Output.GCCErrorList
{
    public static class GCCErrorGenerator
    {
        private static ErrorListProvider _errorListProvider;

        private static readonly HashSet<GCCErrorListItem> CurrentListItems = new HashSet<GCCErrorListItem>();

        private static readonly IVsUIShellOpenDocument VsUiShellOpenDocument =
            Package.GetGlobalService(typeof(IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;

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

        private static void AddTask(GCCErrorListItem item, TaskErrorCategory category)
        {
            try
            {
                // Visual studio has a bug of showing each gcc message twice. Make sure this error list item doesn't exist already
                if (item == null || CurrentListItems.Contains(item)) return;

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
                    case GCCErrorType.NoDetails:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _errorListProvider.Tasks.Add(task);
                CurrentListItems.Add(item);
            }
            catch (Exception e)
            {
                // eat it
                Log.LogError(e.ToString());
            }
        }

        private static void TaskOnNavigate(object sender, EventArgs eventArgs)
        {
            var task = sender as TaskListItem;
            if (task == null) throw new ArgumentException("sender");
            task.Line++; // Navigation starts counting from 1, do ++
            _errorListProvider.Navigate(task, new Guid(Constants.vsViewKindCode));
            task.Line--; // Back to normal, do --
        }

        private static IVsUIHierarchy GetItemHierarchy(string filename)
        {
            if (VsUiShellOpenDocument != null)
            {
                var logicalView = VSConstants.LOGVIEWID_Code;

                if (ErrorHandler.Succeeded(VsUiShellOpenDocument.OpenDocumentViaProject(filename, ref logicalView,
                    out _,
                    out var hierarchy,
                    out _,
                    out _)))
                    return hierarchy;
            }

            return null;
        }

        public static IVsHierarchy GetProjectHierarchy(Project project)
        {
            // Get the vs solution
            var solution = (IVsSolution) Package.GetGlobalService(typeof(IVsSolution));
            var hr = solution.GetProjectOfUniqueName(project.UniqueName, out var hierarchy);

            if (hr == VSConstants.S_OK) return hierarchy;

            return null;
        }


        private static Project GetProjectByNumber(int number)
        {
            var dte = (DTE2) Package.GetGlobalService(typeof(DTE));
            return dte.Solution.Projects.Item(number);
        }

        private static string GetFileByProjectNumber(int number, string filename)
        {
            var proj = GetProjectByNumber(number);
            var projectPath = Path.GetDirectoryName(proj.FileName) ?? string.Empty;

            foreach (var file in Directory.EnumerateFiles(projectPath, "*.*", SearchOption.AllDirectories))
                if (Path.GetFileName(file) == filename)
                    return file;

            return filename;
        }
    }
}