using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using StringFormatter.Services.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace StringFormatter.Models
{
    public class DropHandlerClass : IDropTarget
    {
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            //PupilViewModel sourceItem = dropInfo.Data as PupilViewModel;
            //SchoolViewModel targetItem = dropInfo.TargetItem as SchoolViewModel;
            var collection = dropInfo.TargetCollection as IEnumerable<IStringFormatter>;
            var formatter = dropInfo.Data as IStringFormatter;

            if (formatter != null && collection != null)
            {
                if (IsSame(dropInfo))
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Move;
                }
                else
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Copy;
                }
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            //IStringFormatter sourceItem = (IStringFormatter)dropInfo.Data;
            //Collection<IStringFormatter> targetcollection = (Collection<IStringFormatter>)dropInfo.TargetCollection;
            //targetcollection.Add(sourceItem);

            var insertIndex = GetInsertIndex(dropInfo);
            var destinationList = dropInfo.TargetCollection.TryGetList();
            var data = ExtractData(dropInfo.Data).OfType<object>().ToList();
            bool isSameCollection = false;

            var copyData = ShouldCopyData(dropInfo);
            if (!copyData)
            {
                var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
                if (sourceList != null)
                {
                    isSameCollection = sourceList.IsSameObservableCollection(destinationList);
                    if (!isSameCollection)
                    {
                        foreach (var o in data)
                        {
                            var index = sourceList.IndexOf(o);
                            if (index != -1)
                            {
                                sourceList.RemoveAt(index);

                                // If source is destination too fix the insertion index
                                if (destinationList != null && ReferenceEquals(sourceList, destinationList) && index < insertIndex)
                                {
                                    --insertIndex;
                                }
                            }
                        }
                    }
                }
            }

            if (destinationList != null)
            {
                var objects2Insert = new List<object>();

                // check for cloning
                var cloneData = dropInfo.Effects.HasFlag(DragDropEffects.Copy) || dropInfo.Effects.HasFlag(DragDropEffects.Link);

                foreach (var o in data)
                {
                    var obj2Insert = o;
                    if (cloneData)
                    {
                        if (o is ICloneable cloneable)
                        {
                            obj2Insert = cloneable.Clone();
                        }
                    }

                    objects2Insert.Add(obj2Insert);

                    if (!cloneData && isSameCollection)
                    {
                        var index = destinationList.IndexOf(o);
                        if (index != -1)
                        {
                            if (insertIndex > index)
                            {
                                insertIndex--;
                            }

                            Move(destinationList, index, insertIndex++);
                        }
                    }
                    else
                    {
                        destinationList.Insert(insertIndex++, obj2Insert);
                    }
                }

                SelectDroppedItems(dropInfo, objects2Insert);
            }
        }

        public static void SelectDroppedItems([NotNull] IDropInfo dropInfo, [NotNull] IEnumerable items, bool applyTemplate = true, bool focusVisualTarget = true)
        {
            if (dropInfo == null) throw new ArgumentNullException(nameof(dropInfo));
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (dropInfo.VisualTarget is ItemsControl itemsControl)
            {
                var tvItem = dropInfo.VisualTargetItem as TreeViewItem;
                var tvItemIsExpanded = tvItem != null && tvItem.HasHeader && tvItem.HasItems && tvItem.IsExpanded;

                var itemsParent = tvItemIsExpanded
                    ? tvItem
                    : dropInfo.VisualTargetItem != null
                        ? ItemsControl.ItemsControlFromItemContainer(dropInfo.VisualTargetItem)
                        : itemsControl;
                itemsParent ??= itemsControl;

                (dropInfo.DragInfo.VisualSourceItem as TreeViewItem)?.ClearSelectedItems();
                itemsParent.ClearSelectedItems();

                var selectDroppedItems = dropInfo.VisualTarget is TabControl || (dropInfo.VisualTarget != null && DragDrop.GetSelectDroppedItems(dropInfo.VisualTarget));
                if (selectDroppedItems)
                {
                    foreach (var item in items)
                    {
                        if (applyTemplate)
                        {
                            // call ApplyTemplate for TabItem in TabControl to avoid this error:
                            //
                            // System.Windows.Data Error: 4 : Cannot find source for binding with reference
                            var container = itemsParent.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                            container?.ApplyTemplate();
                        }

                        itemsParent.SetItemSelected(item, true);
                    }

                    if (focusVisualTarget)
                    {
                        itemsControl.Focus();
                    }
                }
            }
        }


        protected static void Move(IList list, int sourceIndex, int destinationIndex)
        {
            if (!list.IsObservableCollection())
            {
                throw new ArgumentException("ObservableCollection<T> was expected", nameof(list));
            }

            if (sourceIndex != destinationIndex)
            {
                var method = list.GetType().GetMethod("Move", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                _ = method?.Invoke(list, new object[] { sourceIndex, destinationIndex });
            }
        }

        public static bool IsSame(IDropInfo dropInfo)
        {
            return dropInfo.DragInfo.SourceCollection.TryGetList().IsSameObservableCollection(dropInfo.TargetCollection.TryGetList());
        }
        public static bool ShouldCopyData(IDropInfo dropInfo)
        {
            // default should always the move action/effect
            if (dropInfo?.DragInfo == null)
            {
                return false;
            }
            return !IsSame(dropInfo);
        }

        protected static int GetInsertIndex(IDropInfo dropInfo)
        {
            var insertIndex = dropInfo.UnfilteredInsertIndex;

            if (dropInfo.VisualTarget is ItemsControl itemsControl)
            {
                if (itemsControl.Items is IEditableCollectionView editableItems)
                {
                    var newItemPlaceholderPosition = editableItems.NewItemPlaceholderPosition;
                    if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning && insertIndex == 0)
                    {
                        ++insertIndex;
                    }
                    else if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd && insertIndex == itemsControl.Items.Count)
                    {
                        --insertIndex;
                    }
                }
            }

            return insertIndex;
        }

        public static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable enumerable and not string)
            {
                return enumerable;
            }

            return Enumerable.Repeat(data, 1);
        }
    }
}
