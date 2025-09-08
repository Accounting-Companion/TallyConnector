using DemoDesktopApp.Converters;
using DemoDesktopApp.Views.Helpers;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using TallyConnector.Core.Models;
using TallyConnector.Core.Models.TallyComplexObjects;
using TallyConnector.Models.Base;

namespace DemoDesktopApp.Extensions;
public static class DataGridExtensions
{
    public static bool GetEnableAutoNesting(DataGrid dataGrid) => (bool)dataGrid.GetValue(EnableAutoNestingProperty);
    public static void SetEnableAutoNesting(DataGrid dataGrid, bool value) => dataGrid.SetValue(EnableAutoNestingProperty, value);
    // Using a DependencyProperty as the backing store for EnableAutoNesting.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EnableAutoNestingProperty =
        DependencyProperty.RegisterAttached("EnableAutoNesting", typeof(bool), typeof(DataGridExtensions), new PropertyMetadata(false, OnEnableAutoNestingChanged));

    private static void OnEnableAutoNestingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid dataGrid) return;

        if ((bool)e.NewValue)
        {
            dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
        }
        else
        {
            dataGrid.AutoGeneratingColumn -= DataGrid_AutoGeneratingColumn;
        }
    }

    private static void DataGrid_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(TallyConnector.Models.Base.Masters.BaseAliasedMasterObject.Name) or nameof(Voucher.VoucherType):
                e.Column.DisplayIndex = 0;
                break;
            case nameof(Voucher.VoucherNumber):
                e.Column.DisplayIndex = 1;
                break;
            default:
                break;
        }

        if (IsCollectionOfSimpleTypes(e.PropertyType))
        {
            // e.Cancel = true;
            if (e.PropertyType.GenericTypeArguments.Any(c => c.IsPrimitive || c == typeof(string)))
            {
                e.Column = CreateSimpleCollectionColumn(e.PropertyName);
                return;
            }
            if (sender is not DataGrid dataGrid) return;

            if (dataGrid.RowDetailsTemplate != null) return;
            string heading = $"{e.PropertyType.DeclaringType!.Name}-{e.PropertyName}";
            e.Column = CreateDetailsButtonColumn(e.PropertyName);
            return;
        }
        if (!IsSimpleType(e.PropertyType))
        {
            e.Column = CreateDetailsButtonColumn(e.PropertyName,
                                                 typeof(ITallyComplexObject).IsAssignableFrom(e.PropertyType));
        }
    }
    /// <summary>
    /// Determines if a type is a collection of simple types (e.g., List<string>, int[], etc.).
    /// </summary>
    private static bool IsCollectionOfSimpleTypes(Type type)
    {
        Type? itemType = GetTypeFromIEnumerable(type);


        // --- Step 3: Check if the Discovered Item DTOType is Simple ---

        // If we couldn't determine an item type, we can't proceed.
        if (itemType == null)
        {
            return false;
        }

        // Now, we reuse our other helper method to check if the item type itself is simple.
        // This is the elegant part - we compose our helper methods.
        return IsSimpleType(itemType);
    }

    private static Type? GetTypeFromIEnumerable(Type type)
    {   // A) If it's not a collection at all, it can't be a collection of simple types.
        //    We also exclude 'string' itself, because although it's an IEnumerable<char>,
        //    we want to treat it as a single simple type.
        if (!typeof(IEnumerable).IsAssignableFrom(type) || type == typeof(string))
        {
            return null;
        }
        Type? itemType = null;

        // A) Is it an array (like int[] or string[])?
        if (type.IsArray)
        {
            // GetElementType() is the .NET way to get the type of items in an array.
            // For int[], it returns typeof(int).
            itemType = type.GetElementType();
        }
        // B) Is it a generic collection (like List<T> or ObservableCollection<T>)?
        else if (type.IsGenericType && type.GetGenericArguments().Length == 1)
        {
            // GetGenericArguments() returns an array of the type parameters.
            // For List<string>, it returns an array containing one item: typeof(string).
            itemType = type.GetGenericArguments()[0];
        }

        return itemType;
    }

    private static bool IsSimpleType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);


        if (underlyingType != null)
        {
            type = underlyingType;
        }

        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid);
    }

    private static DataTemplate? CreateNestedDataGridTemplateWithFactory(string propertyName, bool isTallyComplexObject)
    {
        var dataTemplate = new DataTemplate();

        // 1. Create a factory for our UserControl
        var controlFactory = new FrameworkElementFactory(typeof(NestedDataGrid));

        // 2. Create a binding that connects the current row's collection property (e.g., "Orders")
        //    to our UserControl's ItemsSource dependency property.
        var itemsSourceBinding = new Binding(propertyName)
        {
            Converter = new ObjectToCollectionConverter()
        };
        controlFactory.SetBinding(NestedDataGrid.ItemsSourceProperty, itemsSourceBinding);
        controlFactory.SetValue(NestedDataGrid.PropertyNameProperty, propertyName);
        if (isTallyComplexObject)
        {
            controlFactory.SetBinding(NestedDataGrid.TallyObjectProperty, new Binding(propertyName));
        }
        // 3. Set the factory as the root of the template's visual tree
        dataTemplate.VisualTree = controlFactory;
        dataTemplate.Seal();

        return dataTemplate;


    }

    private static DataGridTemplateColumn CreateDetailsButtonColumn(string propertyName, bool isTallyComplexObject = false)
    {
        var column = new DataGridTemplateColumn
        {
            Header = propertyName, // Use the property name as the column header
            CellTemplate = CreateNestedDataGridTemplateWithFactory(propertyName, isTallyComplexObject)
        };
        return column;
    }


    private static DataGridTemplateColumn CreateSimpleCollectionColumn(string propertyName)
    {
        var dataTemplate = new DataTemplate();

        // 1. Create a factory for our UserControl
        var controlFactory = new FrameworkElementFactory(typeof(TextList));

        // 2. Create a binding that connects the current row's collection property (e.g., "Orders")
        //    to our UserControl's ItemsSource dependency property.
        var itemsSourceBinding = new Binding(propertyName);
        controlFactory.SetBinding(TextList.ItemsSourceProperty, itemsSourceBinding);
        controlFactory.SetValue(NestedDataGrid.PropertyNameProperty, propertyName);
        // 3. Set the factory as the root of the template's visual tree
        dataTemplate.VisualTree = controlFactory;
        dataTemplate.Seal();

        var column = new DataGridTemplateColumn
        {
            Header = propertyName, // Use the property name as the column header
            CellTemplate = dataTemplate
        };
        return column;


    }
}
