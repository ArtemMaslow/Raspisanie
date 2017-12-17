
namespace SozdanieRaspisaniya
{
    public class RepresentationSubject<Tkey, KSubject>
    {
        Tkey Key { get; set; }
        KSubject[] Values { get; set; }
    }
}
