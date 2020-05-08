﻿using Gtk;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Reflection;
using LogikUI.Component;
using LogikUI.Hierarchy;
using LogikUI.Circuit;
using LogikUI.Util;
<<<<<<< HEAD
=======
using System.Globalization;
using System.Reflection;
using LogikUI.Interop;
>>>>>>> upstream/master
using LogikUI.Simulation;
using LogikUI.Toolbar;
using LogikUI.Simulation.Gates;
using LogikUI.File;

namespace LogikUI
{
    class Program
    {
        public static Data Backend;
        
        static Gtk.Toolbar CreateToolbar(CircuitEditor editor) {
            Gtk.Toolbar toolbar = new Gtk.Toolbar();

            SelectTool selectTool = new SelectTool(editor, toolbar);
            WireTool wireTool = new WireTool(editor, toolbar);
            // FIXME: Make this be selected with a callback or something
            //editor.CurrentTool = selectTool;

            // FIXME: We don't want to new the components here!!
            ComponentTool bufferTool = new ComponentTool(ComponentType.Buffer, "Buffer gate", editor, toolbar);

            ComponentTool andTool = new ComponentTool(ComponentType.And, "And gate", editor, toolbar);

            ComponentTool orTool = new ComponentTool(ComponentType.Or, "Or gate", editor, toolbar);

            ComponentTool xorTool = new ComponentTool(ComponentType.Xor, "Xor gate", editor, toolbar);

            SeparatorToolItem sep = new SeparatorToolItem();

            toolbar.Insert(selectTool, 0);
            toolbar.Insert(wireTool, 1);
            toolbar.Insert(sep, 2);
            toolbar.Insert(bufferTool, 3);
            toolbar.Insert(andTool, 4);
            toolbar.Insert(orTool, 5);
            toolbar.Insert(xorTool, 6);

            return toolbar;
        }

        static void AddFilters(FileChooserDialog fcd)
        {
            FileFilter filter = new FileFilter();
            filter.Name = "Project Files";
            filter.AddPattern("*.xml");
            filter.AddPattern("*.XML");

            FileFilter all = new FileFilter();
            all.Name = "All files";
            all.AddPattern("*");

            fcd.AddFilter(filter);
            fcd.AddFilter(all);
        }

        static MenuBar CreateMenuBar(Window parent) 
        {
            MenuItem open = new MenuItem("Open...");
            open.Activated += (object? sender, EventArgs e) =>
            {
                FileChooserDialog fcd = new FileChooserDialog("Open Project", parent, FileChooserAction.Open,
                    Stock.Open, ResponseType.Ok,
                    Stock.Cancel, ResponseType.Cancel);
                fcd.SelectMultiple = false;
                AddFilters(fcd);

                if (fcd.Run() == (int)ResponseType.Ok)
                {
                    try
                    {
                        FileManager.Load(fcd.Filename);

                        Console.WriteLine($"Successfully read and parsed project file { fcd.Filename }.");
                        Console.WriteLine($"- wires: { FileManager.Wires }");
                        Console.WriteLine($"- components: { FileManager.Components }");
                        Console.WriteLine($"- labels: { FileManager.Labels }");
                    }
                    catch (Exception err)
                    {
                        // FIXME: Do something if parse fails?

                        Output.WriteError($"Failed to read and parse project file { fcd.Filename }.", err);
                    }
                }

                fcd.Dispose();
            };

            MenuItem saveAs = new MenuItem("Save As...");
            saveAs.Activated += (object? sender, EventArgs e) =>
            {
                FileChooserDialog fcd = new FileChooserDialog("Save Project", parent, FileChooserAction.Open,
                    Stock.Save, ResponseType.Ok,
                    Stock.Cancel, ResponseType.Cancel);
                AddFilters(fcd);

                if (fcd.Run() == (int)ResponseType.Ok)
                {
                    try
                    {
                        FileManager.Save(fcd.Filename);

                        Console.WriteLine($"Successfully saved and parsed project file { fcd.Filename }.");
                        Console.WriteLine($"- wires: { FileManager.Wires }");
                        Console.WriteLine($"- components: { FileManager.Components }");
                        Console.WriteLine($"- labels: { FileManager.Labels }");
                    }
                    catch (Exception err)
                    {
                        // FIXME: Do something if parse fails?

                        Output.WriteError($"Failed to write and parse project file { fcd.Filename }.", err);
                    }
                }

                fcd.Dispose();
            };

            MenuItem save = new MenuItem("Save...");
            save.Activated += (object? sender, EventArgs e) =>
            {
                if (FileManager.IsNew) saveAs.Activate();
                else FileManager.Save();
            };

            Menu fileMenu = new Menu();
            fileMenu.Append(open);
            fileMenu.Append(save);
            fileMenu.Append(saveAs);

            MenuItem file = new MenuItem("File");
            file.AddEvents((int)Gdk.EventMask.AllEventsMask);
            file.Submenu = fileMenu;

            // FIXME: Hook up callbacks

            // FIXME: On windows (and Mac) there should be no delay
            // on the ability to close the menu when you've opened it.
            // Atm there is a delay after opening the menu and when
            // you can close it...

            MenuBar bar = new MenuBar();
            bar.Append(file);

            return bar;
        }

        // VERY IMPORTANT!!!!!!!!!!!!
        // After the call to Application.Init() NullReferenceExceptions
        // will no longer be thrown. This is an active bug in GtkSharp
        // and can be tracked here https://github.com/GtkSharp/GtkSharp/issues/155
        // Hopefully this can be fixed sooner rather than later...
        static void Main()
        {
            Backend = Logic.Init();
            
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Value a = new Value(0b00_00_00_00_01_01_01_01_10_10_10_10_11_11_11_11, 16);
            Value b = new Value(0b00_01_10_11_00_01_10_11_00_01_10_11_00_01_10_11, 16);
            Console.WriteLine($"Resolve: {Value.Resolve(a, b)}");
            Console.WriteLine($"And: {Value.And(a, b)}");
            Console.WriteLine($"Or: {Value.Or(a, b)}");
            Console.WriteLine($"Not: {Value.Not(a)}");

            Application.Init();

            GLib.ExceptionManager.UnhandledException += ExceptionManager_UnhandledException;
            
            Window wnd = new Window("Logik");
            wnd.Resize(1600, 800);

            Notebook nbook = new Notebook();
            var circuitEditor = new CircuitEditor();
            nbook.AppendPage(circuitEditor.DrawingArea, new Label("Circuit editor"));
            nbook.AppendPage(new Label("TODO: Package editor"), new Label("Package editor"));

            Notebook sideBar = new Notebook();
            var components = new ComponentView(new List<ComponentFolder> { 
                new ComponentFolder("Test folder 1", new List<Component.Component>()
                {
                    new Component.Component("Test comp 1", "x-office-document"),
                    new Component.Component("Test comp 2", "x-office-document"),
                    new Component.Component("Test comp 3", "x-office-document"),
                }),
                new ComponentFolder("Test folder 2", new List<Component.Component>()
                {
                    new Component.Component("Another test comp 1", "x-office-document"),
                    new Component.Component("Another test comp 2", "x-office-document"),
                    new Component.Component("Another test comp 3", "x-office-document"),
                }),
            });
            sideBar.AppendPage(components.TreeView, new Label("Components"));
            var hierarchy = new HierarchyView(new HierarchyComponent("Top comp", "x-office-document", new List<HierarchyComponent>()
            {
                new HierarchyComponent("Test Comp 1", "x-office-document", new List<HierarchyComponent>(){
                    new HierarchyComponent("Test Nested Comp 1", "x-office-document", new List<HierarchyComponent>()),
                }),
                new HierarchyComponent("Test Comp 2", "x-office-document", new List<HierarchyComponent>(){
                    new HierarchyComponent("Test Nested Comp 1", "x-office-document", new List<HierarchyComponent>()),
                    new HierarchyComponent("Test Nested Comp 2", "x-office-document", new List<HierarchyComponent>()),
                }),
                new HierarchyComponent("Test Comp 3", "x-office-document", new List<HierarchyComponent>()),
            }));
            sideBar.AppendPage(hierarchy.TreeView, new Label("Hierarchy"));

            HPaned hPaned = new HPaned();
            hPaned.Pack1(sideBar, false, false);
            hPaned.Pack2(nbook, true, false);

            //Add the label to the form
            VBox box = new VBox(false, 0);
            box.PackStart(CreateMenuBar(wnd), false, false, 0);
            box.PackStart(CreateToolbar(circuitEditor), false, false, 0);
            box.PackEnd(hPaned, true, true, 0);
            box.Expand = true;
            wnd.Add(box);

            wnd.Destroyed += Wnd_Destroyed;

            wnd.ShowAll();
            Application.Run();
        }

        private static void ExceptionManager_UnhandledException(GLib.UnhandledExceptionArgs args)
        {
            if (args.ExceptionObject is Exception e)
                if (e is TargetInvocationException tie && tie.InnerException != null) throw tie.InnerException;
                else throw e;
            else throw new Exception($"We got a weird exception! {args.ExceptionObject}");
        }

        private static void Wnd_Destroyed(object? sender, EventArgs e)
        {
            Logic.Exit(Backend);
            Application.Quit();
        }
    }
}
