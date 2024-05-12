﻿using System;
using System.Linq;
using plannerCLI.Repositories;
using plannerCLI.Models;

namespace plannerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteDatabase db = new SQLiteDatabase();
            TaskRepository taskRepository = new TaskRepository();

            // Ensure the database and table are properly set up.
            db.CreateStandardTasksTable();

            // Handle specific commands
            if (args.Length > 0)
            {
                if (args[0].Equals("add", StringComparison.OrdinalIgnoreCase))
                {
                    HandleAddTask(args.Skip(1).ToArray(), taskRepository);
                }
                else if (args[0].Equals("delete", StringComparison.OrdinalIgnoreCase) ||
                         args[0].Equals("remove", StringComparison.OrdinalIgnoreCase) ||
                         args[0].Equals("complete", StringComparison.OrdinalIgnoreCase) ||
                         args[0].Equals("finish", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length > 1 && int.TryParse(args[1], out int taskId))
                    {
                        taskRepository.DeleteTask(taskId);
                    }
                    else
                    {
                        Console.WriteLine("Error occured, possible cause: Please provide a valid task ID.");
                    }
                }
                else if (args[0] == "-h" || args[0] == "-H" || args[0] == "--help" || args[0] == "--Help")
                {
                    PrintHelp();
                }
                else
                {
                    DisplayTasksOrDefaultToHelp(taskRepository);
                }
            }
            else
            {
                DisplayTasksOrDefaultToHelp(taskRepository);
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("plannerCLI: by David Reese (eseer-divad)");
            Console.WriteLine("Description: A command line task management / day planner application.");
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine("Commands:");
            Console.WriteLine("`plannercli`            | View Task List");
            Console.WriteLine("`plannercli -h`         | View This Help Page");
            Console.WriteLine();
            Console.WriteLine("`plannercli add [options]` | Add Task to List");
            Console.WriteLine("Options for 'add':");
            Console.WriteLine("  -t, --task [taskname]   | Specify the task name.");
            Console.WriteLine("  -p, --priority [level]  | Specify the task priority.");
            Console.WriteLine("  -d, --due [duedate]     | Specify the due date (format YYYY-MM-DD).");
            Console.WriteLine("  -n, --note [note]       | Specify additional notes.");
            Console.WriteLine("---------------------------------------------------------------------------");
        }

        static void HandleAddTask(string[] args, TaskRepository taskRepository)
        {
            string taskName = null, due = null, note = null;
            int priority = -1;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-t":
                    case "--task":
                        taskName = args[++i];
                        break;
                    case "-p":
                    case "--priority":
                        if (int.TryParse(args[++i], out int parsedPriority))
                        {
                            priority = parsedPriority;
                        }
                        else
                        {
                            Console.WriteLine("Invalid priority. Please enter a valid integer.");
                            return;
                        }
                        break;
                    case "-d":
                    case "--due":
                        due = args[++i];
                        break;
                    case "-n":
                    case "--note":
                        note = args[++i];
                        break;
                    default:
                        Console.WriteLine($"Unknown argument: {args[i]}");
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(taskName))
            {
                Console.WriteLine("Task name is required.");
                return;
            }

            var task = new StandardTaskModel
            {
                TaskName = taskName,
                Due = due,
                Priority = priority,
                Note = note,
                Added = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            taskRepository.AddTask(task);
            Console.WriteLine("Task added successfully!");
        }

        static void DisplayTasksOrDefaultToHelp(TaskRepository taskRepository)
        {
            var tasks = taskRepository.GetTasks();
            if (tasks.Count > 0)
            {
                Console.WriteLine("Tasks:");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("| ID | Task Name | Due Date | Priority | Note |");
                Console.WriteLine("----------------------------------------------");

                foreach (var task in tasks)
                {
                    Console.WriteLine($"| {task.Id.ToString().PadRight(3)} | {task.TaskName.PadRight(10)} | {task.Due.PadRight(9)} | {task.Priority.ToString().PadRight(8)} | {task.Note.PadRight(5)} |");
                }

                Console.WriteLine("----------------------------------------------");
            }
            else
            {
                PrintHelp(); // Print help if no tasks are found
            }
        }
    }
}