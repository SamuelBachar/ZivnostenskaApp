using SharedTypesLibrary.DTOs.Bidirectional.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace A.ViewModels;

internal class CategoryViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private Color _frameBackgroundColor = Colors.Transparent;

    public Color FrameBackgroundColor
    {
        get => _frameBackgroundColor;
        set
        {
            _frameBackgroundColor = value;
            OnPropertyChanged();
        }
    }

    public CategoryDTO CategoryData { get; private set; }

    public CategoryViewModel(CategoryDTO category)
    {
        CategoryData = category;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {  
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
