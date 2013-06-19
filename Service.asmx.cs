using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace TurfWars
{
    /// <summary>
    /// Summary description for Service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Service : System.Web.Services.WebService
    {

        [WebMethod]
        public System.Xml.XmlDocument addClaim(string user, double lat, double lon)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            Geo.Box box = TurfWars.Global.game.addClaim(user, lat, lon);

            System.Xml.XmlElement tile = doc.CreateElement("tile");
            tile.SetAttribute("north", box.north.ToString());
            tile.SetAttribute("south", box.south.ToString());
            tile.SetAttribute("west", box.west.ToString());
            tile.SetAttribute("east", box.east.ToString());
            
            doc.AppendChild(tile);
            return doc;
        }

        [WebMethod]
        public System.Xml.XmlDocument getUserStats(string user)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            TurfWarsGame.User u = TurfWars.Global.game.getUser(user);
            
            System.Xml.XmlElement xmlUser = doc.CreateElement("user");
            xmlUser.SetAttribute("name", user);
            xmlUser.SetAttribute("balance", u.getAccountBalance().ToString());
            xmlUser.SetAttribute("shares", u.shares.ToString());

            doc.AppendChild(xmlUser);
            return doc;
        }

        [WebMethod]
        public System.Xml.XmlDocument getClaims()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            System.Xml.XmlElement claims = doc.CreateElement("claims");

            foreach(TurfWarsGame.Claim c in TurfWars.Global.game.getClaims())
            {
                System.Xml.XmlElement claim = doc.CreateElement("claim");

                System.Xml.XmlElement owners = doc.CreateElement("owners");
                foreach (string u in c.getOwners().Keys)
                {
                    System.Xml.XmlElement owner = doc.CreateElement("owner");
                    owner.SetAttribute("name", u);
                    owner.SetAttribute("share", c.getOwners()[u].ToString());
                    owners.AppendChild(owner);
                }

                System.Xml.XmlElement tile = doc.CreateElement("tile");
                tile.SetAttribute("north",c.box.north.ToString());
                tile.SetAttribute("south",c.box.south.ToString());
                tile.SetAttribute("west", c.box.west.ToString());
                tile.SetAttribute("east", c.box.east.ToString());
                claim.AppendChild(owners);
                claim.AppendChild(tile);
                claims.AppendChild(claim);
            }

            doc.AppendChild(claims);
            return doc;
        }
    }
}
