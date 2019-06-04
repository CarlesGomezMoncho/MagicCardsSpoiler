
public class Card
{
    public int id;
    public int cardNum;
    public string name;
    public int set;
    public string image;
    public string type;
    public string color;
    public string rarity;
    public string date;

    public Card(int id, int cardNum, string name, string image, int set, string type, string color, string rarity, string date)
    {
        this.id = id;
        this.cardNum = cardNum;
        this.name = name;
        this.image = image;
        this.set = set;
        this.type = type;
        this.color = color;
        this.rarity = rarity;
        this.date = date;
    }

    override
    public string ToString()
    {
        return "id: " + id + "num: " + cardNum + "  name: " + name + "  image: " + image + "  set: " + set + "  type: " + type + "  color: " + color + "  rarity: " + rarity + "  date: " + date;
    }
}
