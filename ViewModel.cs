using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace ADO_ToDoList
{
    class ViewModel : INotifyPropertyChanged
    {
        Model mdl;
        ICommand cmdAddNew;
        ICommand cmdUpdateCurrent;
        ICommand cmdAllowEditCurrent;
        ICommand cmdRemoveCurrent;
        ICommand cmdCommitChanges;
        ICommand cmdReloadTasks;
        TaskEntity selectedTaskEntity = null;
        Boolean isAllowEditCurrentCmdActivated = false;

        public ViewModel()
        {
            mdl = new Model();
            cmdAddNew = new AddNewCommandImplementation(this);
            cmdUpdateCurrent = new UpdateCurrentCommandImplementation(this);
            cmdAllowEditCurrent = new AllowEditCurrentCommandImplementation(this);
            cmdRemoveCurrent = new RemoveCurrentCommandImplementation(this);
            cmdCommitChanges = new CommitChangesCommandImplementation(this);
            cmdReloadTasks = new ReloadTasksCommandImplementation(this);
        }

        public TaskEntity SelectedTaskEntity
        {
            get { return selectedTaskEntity; }
            set { selectedTaskEntity = value;
            IsAllowEditCurrentCmdActivated = false;
                OnPropertyChanged("SelectedTaskEntity");
            }
        }

        public ICommand CmdReloadTasks
        {
            get { return cmdReloadTasks; }
        }

        public ICommand CmdCommitChanges
        {
            get { return cmdCommitChanges; }
        }

        //Обновить текущий в БД
        public ICommand CmdUpdateCurrent
        {
            get { return cmdUpdateCurrent; }
        }

        public ICommand CmdRemoveCurrent
        {
            get { return cmdRemoveCurrent; }
        }

        //Разрешить редактирование текущего
        public ICommand CmdAllowEditCurrent
        {
            get { return cmdAllowEditCurrent; }
        }

        public ICommand CmdAddNew
        {
            get { return cmdAddNew; }
        }

        public Boolean IsAllowEditCurrentCmdActivated
        {
            set { isAllowEditCurrentCmdActivated = value;
                OnPropertyChanged("IsAllowEditCurrentCmdActivated");
            }
            get { return isAllowEditCurrentCmdActivated; }
        }

        public Model Mdl
        {
            get { return mdl; }
        }

        class ReloadTasksCommandImplementation : ICommand
        {
            ViewModel vMdl;

            public ReloadTasksCommandImplementation(ViewModel vMdl)
            {
                this.vMdl = vMdl;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                vMdl.Mdl.InitializeTaskEntitiesCollection();
            }
        }

        class CommitChangesCommandImplementation : ICommand
        {
            ViewModel vMdl;

            public CommitChangesCommandImplementation(ViewModel vMdl)
            {
                this.vMdl = vMdl;
            }

            public bool CanExecute(object parameter)
            {
                bool canExecute = false;
                foreach (TaskEntity item in vMdl.Mdl.TaskEntities)
                    if (canExecute = item.IsCommitNeeded) break;
                return canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                foreach (TaskEntity item in vMdl.Mdl.TaskEntities)
                    if (item.IsCommitNeeded)
                    {
                        item.UpdateInTable(vMdl.Mdl.ConnectionStr, vMdl.Mdl.SqlTable);
                        item.IsCommitNeeded = false;
                    }
            }
        }

        class RemoveCurrentCommandImplementation : ICommand
        {
            ViewModel vMdl;

            public RemoveCurrentCommandImplementation(ViewModel vMdl)
            {
                this.vMdl = vMdl;
            }

            public bool CanExecute(object parameter)
            {
                return vMdl.SelectedTaskEntity != null;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                vMdl.SelectedTaskEntity.RemoveFromTable(vMdl.Mdl.ConnectionStr, vMdl.Mdl.SqlTable);
                vMdl.Mdl.TaskEntities.Remove(vMdl.SelectedTaskEntity);
            }
        }

        class AllowEditCurrentCommandImplementation : ICommand
        {
            ViewModel vMdl;

            public AllowEditCurrentCommandImplementation(ViewModel vMdl)
            {
                this.vMdl = vMdl;
            }

            public bool CanExecute(object parameter)
            {
                return !vMdl.IsAllowEditCurrentCmdActivated && vMdl.SelectedTaskEntity != null;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                vMdl.IsAllowEditCurrentCmdActivated = true;
            }
        }

        class UpdateCurrentCommandImplementation : ICommand
        {
            ViewModel vMdl;

            public UpdateCurrentCommandImplementation(ViewModel vMdl)
            {
                this.vMdl = vMdl;
            }

            public bool CanExecute(object parameter)
            {
                return vMdl.SelectedTaskEntity != null;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                vMdl.SelectedTaskEntity.UpdateInTable(vMdl.Mdl.ConnectionStr, vMdl.Mdl.SqlTable);
                vMdl.SelectedTaskEntity.IsCommitNeeded = false;
            }
        }

        class AddNewCommandImplementation : ICommand
        {
            ViewModel vMdl;

            public AddNewCommandImplementation(ViewModel vMdl)
            {
                this.vMdl = vMdl;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                AddTaskDialog addTaskDlg = new AddTaskDialog(DateTime.Now, vMdl.Mdl.MinPriority, vMdl.Mdl.MaxPriority);
                TaskEntity taskEntity = addTaskDlg.ShowDialog();
                if (taskEntity != null)
                {
                    int identity = taskEntity.AddToTable(vMdl.Mdl.ConnectionStr, vMdl.Mdl.SqlTable);
                    taskEntity.ID = identity;
                    vMdl.Mdl.TaskEntities.Add(taskEntity);
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
            
        }
    }
}
