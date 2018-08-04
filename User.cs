using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Place.Users
{
    [XmlRoot(ElementName = "user")]
    public class User
    {
        #region Constructor

        public User() { }

        public User(string Name, string Admin) {
            this.Name = Name;
            this.Admin = Admin;
        }

        #endregion

        #region Properties

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "admin")]
        public string Admin { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the users username
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Name;
        }

        /// <summary>
        /// Returns true if the user is an admin
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin() {
            return Admin == "true";
        }

        /// <summary>
        /// Sets the users Access list
        /// </summary>
        /// <param name="path">Path to the xml document</param>
        public Dictionary<string, string> GetAccessList(string path) {
            //List to be returned
            Dictionary<string, string> accessList = null;
            //Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Select all the user nodes
            XmlNodeList nodes = doc.SelectNodes("//users/user");

            for (int i = 0; i < nodes.Count; i++) {
                XmlNode _username = nodes[i].SelectSingleNode("name");
                //Check if the username is the same as the current user
                if (_username.InnerText == Name) {
                    accessList = new Dictionary<string, string>();
                    //Get the node "access" and get all children 
                    XmlNode accessNode = nodes[i].SelectSingleNode("access");
                    XmlNodeList accesses = accessNode.ChildNodes;
                    
                    /*For every child add
                     * the Xml name (e.g. "main")
                     * and the value (e.g. "all")
                     * to a dictionary. */
                    for (int j = 0; j < accesses.Count; j++) {
                        accessList.Add(accesses[j].LocalName, accesses[j].InnerText);
                    }

                    break;
                }
            }
            //Return the dictionary

            return accessList;
        }

        /// <summary>
        /// Serializes the User Class data
        /// </summary>
        /// <param name="path">The path of the XML document to be modified</param>
        public void Serialize(string path) {
            XDocument doc = XDocument.Load(path);
            XElement user = new XElement("user");

            user.Add(new XElement(
                "name",
                Name
                ));
            user.Add(new XElement(
                "admin",
                Admin
                ));

            doc.Element("users").Add(user);
            doc.Save(path);
        }

        /// <summary>
        /// Deletes the current user
        /// </summary>
        /// <param name="path">Path to the XML</param>
        public void Delete(string path) {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList nodes = doc.SelectNodes("//users/user");

            for (int i = 0; i < nodes.Count; i++) {
                XmlNode _username = nodes[i].SelectSingleNode("username");

                if (_username.InnerText == Name) {
                    nodes[i].ParentNode.RemoveChild(nodes[i]);
                    break;
                }
            }

            doc.Save(path);
        }

        #endregion
    }

    [XmlRoot(ElementName = "users")]
    public class UserCollection
    {
        #region Properties

        [XmlElement(ElementName = "user")]
        public List<User> List { get; set; }

        #endregion

        #region Methods

        public static UserCollection Deserialize(string path) {
            UserCollection _users = null;
            XmlSerializer serializer = new
            XmlSerializer(typeof(UserCollection));

            if (File.Exists(path)) {
                StreamReader reader = new StreamReader(path);
                _users = (UserCollection)serializer.Deserialize(reader);
                reader.Close();
            }

            return _users;
        }

        #endregion
    }
}
