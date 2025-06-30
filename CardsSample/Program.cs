using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CardsSample;

public class Program
{
    [STAThread]
    static void Main()
    {
        var window = new Window()
        {
            Background = Brushes.Gray
        };
        var app = new Application();
        app.Resources.MergedDictionaries.Add(
            new ResourceDictionary
            {
                Source = GetResourceUri(typeof(Program).Assembly.GetName().Name, "Themes/generic.xaml")
            });

        Library library = LoadLibrary();

        window.Content = library;

        app.Run(window);
    }

    private static Library LoadLibrary()
    {
        var library = new Library();
        library.Categories.Add(
            new Category("Family")
            {
                new Card("One"),
                new Card("Two"),
                new Card("Three")
            });
        library.Categories.Add(
            new Category("For You")
            {
                new Card("One"),
                new Card("Two"),
                new Card("Three")
            });
        return library;
    }

    public static Uri GetResourceUri(string assemblyName, string xamlName)
    {
        return new Uri(assemblyName + ";component/" + xamlName, UriKind.Relative);
    }
}

public class Card : ObservableObject
{
    public Card(string title)
    {
        Title = title;
        Task.Delay(2000 + Utilities.Rnd(4000)).ContinueWith(_ =>
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Image = Utilities.GetImage();
            });
        });
    }

    private ImageSource image;
    public ImageSource Image
    {
        get => image;
        set => SetField(ref image, value);
    }

    private string title;
    public string Title
    {
        get => title;
        set => SetField(ref title, value);
    }

    private string category;
    public string Category
    {
        get => category;
        set => SetField(ref category, value);
    }

    private string previewUrl;
    public string PreviewUrl
    {
        get => previewUrl;
        set => SetField(ref previewUrl, value);
    }
}

public class Library
{
    public ObservableCollection<Category> Categories { get; } = new();
}

public class Category : ObservableObject, IEnumerable<Card>
{
    public ObservableCollection<Card> Cards { get; } = new();

    private string name;
    public string Name
    {
        get => name;
        set => SetField(ref name, value);
    }

    public Category(string name)
    {
        Name = name;
    }

    public void Add(Card card)
    {
        Cards.Add(card);
    }

    public IEnumerator<Card> GetEnumerator() => ((IEnumerable<Card>)Cards).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Cards).GetEnumerator();
}

public class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetField<TField>(ref TField field, TField newValue, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<TField>.Default.Equals(field, newValue))
        {
            return false;
        }

        field = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        return true;
    }

    protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class Utilities
{
    private static Random Random = new Random(1);
    private static byte RandomByte => (byte)Rnd();

    public static int Rnd(int max = 255) => Random.Next(max);

    public static ImageSource GetImage()
    {
        byte[] pixels = new byte[]
        {
            RandomByte, 0, 0,  // Pixel 0,0: Red
            0, RandomByte, 0,  // Pixel 1,0: Green
            0, 0, RandomByte,  // Pixel 0,1: Blue
            RandomByte, RandomByte, 0 // Pixel 1,1: Yellow
        };

        int width = 2;
        int height = 2;
        int bytesPerPixel = 3;
        int stride = width * bytesPerPixel;

        BitmapSource bitmap = BitmapSource.Create(
            width,
            height,
            96,
            96,
            PixelFormats.Rgb24,
            null, // palette
            pixels,
            stride
        );

        return bitmap;
    }
}
