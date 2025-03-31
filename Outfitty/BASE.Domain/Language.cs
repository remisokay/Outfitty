namespace BASE.Domain;

public class Language : Dictionary<string, string>
{
    private const string DefaultCulture = "en";

    public new string this[string key]
    {
        get => base[key];
        set => base[key] = value;
    }

    public Language(string value) : this(value, Thread.CurrentThread.CurrentUICulture.Name)
    {
    }

    public Language(string value, string culture)
    {
        if (culture.Length < 1) throw new ApplicationException("Language is required!");
        
        var neutralLang = culture.Split('-')[0]; // ("en-US" → "en")
        this[neutralLang] = value;

        if (!ContainsKey(DefaultCulture))
        {
            this[DefaultCulture] = value;
        }
        
    }

    public string? Translate(string? culture = null)
    {
        if (Count == 0) return null;
        culture = culture?.Trim() ?? Thread.CurrentThread.CurrentUICulture.Name;
        
        if (ContainsKey(culture)) return this[culture];
        var neutralLang = culture.Split('-')[0];
        
        if (ContainsKey(neutralLang)) return this[neutralLang];
        if (ContainsKey(DefaultCulture)) return this[DefaultCulture];
        
        return null;
    }

    public void SetTranslation(string value, string? culture = null)
    {
        culture = culture?.Trim() ?? Thread.CurrentThread.CurrentUICulture.Name;
        var neutralLang = culture.Split('-')[0];
        this[neutralLang] = value;
    }

    public override string ToString()
    {
        return Translate() ?? "NOT FOUND";
    }
    
    // string xxx = new Language("foo","et-EE"); xxx == "foo";
    // Lets you use a Language anywhere a string is expected
    public static implicit operator string(Language? langStr) => langStr?.ToString() ?? "null";

    // Language xxx = "foobar";
    // Lets you create a LangStr by just assigning a string
    public static implicit operator Language(string value) => new Language(value);
    
}