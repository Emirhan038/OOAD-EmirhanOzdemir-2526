using System;
using System.Collections.Generic;
using System.Text;

namespace WpfLayoutUIOefenblad.Helpers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class NavPageAttribute : Attribute
{
    public NavPageAttribute()
    {
        this.Title = "title";
        this.Description = "description";
        this.Order = 0;
        this.IsVisible = true;
    }
    public NavPageAttribute(string title, string description, int order)
    {
        this.Title = title;
        this.Description = description;
        this.Order = order;
        this.IsVisible = true;
    }

    public string Title { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public bool IsVisible { get; set; }
}
