using PirateRobotArmyLib;
using PirateRobotArmyLib.AILib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PirateRobotArmy
{
    public partial class AiSelectionForm : Form
    {
        public bool ShouldRunGame { get; private set; }
        public Game Game { get; private set; }

        private Dictionary<TeamInfoAttribute, TypeInfo> ReflectionLoadedAis;

        private Dictionary<string, GameConstants> Constants;

        public AiSelectionForm()
        {
            InitializeComponent();

            ReflectionLoadedAis = new Dictionary<TeamInfoAttribute, TypeInfo>();

            var entry = Assembly.GetEntryAssembly();

            HashSet<Assembly> seenAssemblies = new HashSet<Assembly>();
            Queue<Assembly> pendingAssemblies = new Queue<Assembly>();
            pendingAssemblies.Enqueue(entry);

            while(pendingAssemblies.Count != 0)
            {
                var nextAssembly = pendingAssemblies.Dequeue();
                if (seenAssemblies.Contains(nextAssembly))
                    continue;
                seenAssemblies.Add(nextAssembly);
                foreach(var tuple in ReflectionHelper.FindAllAis(nextAssembly))
                    ReflectionLoadedAis.Add(tuple.Item2, tuple.Item1);
                foreach (var reference in nextAssembly.GetReferencedAssemblies())
                {
                    pendingAssemblies.Enqueue(Assembly.Load(reference.FullName));
                }
            }


            foreach(var item in ReflectionLoadedAis.Keys)
            {
                AllAiBox.Items.Add(item);
            }

            AllAiBox.SelectedIndexChanged += OnSelectionChanged;

            Constants = new Dictionary<string, GameConstants>();

            using (var stream = new StreamReader(typeof(TeamAi).Assembly.GetManifestResourceStream("PirateRobotArmyLib.GameRules.xml")))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(stream.ReadToEnd());

                foreach(var node in doc.SelectNodes("Rules/RuleSet").OfType<XmlNode>())
                {
                    var name = node.Attributes["name"].Value;
                    Constants.Add(name, new GameConstants(node));
                    RulesSelector.Items.Add(name);
                }
            }
            RulesSelector.SelectedIndex = 0;
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (AllAiBox.SelectedItem != null)
            {
                TeamInfoAttribute attr = (TeamInfoAttribute)AllAiBox.SelectedItem;
                NameLabel.Text = attr.Name;
                AuthorLabel.Text = attr.Author;
            }
            else
            {
                NameLabel.Text = "";
                AuthorLabel.Text = "";
            }
        }

        private void ClickAddButton(object sender, EventArgs e)
        {
            if(AllAiBox.SelectedItem != null)
            {
                PresentAiBox.Items.Add(AllAiBox.SelectedItem);
            }
        }

        private void ClickRemoveButton(object sender, EventArgs e)
        {
            if(PresentAiBox.SelectedIndex != -1)
            {
                int index = PresentAiBox.SelectedIndex;
                PresentAiBox.Items.RemoveAt(PresentAiBox.SelectedIndex);
                if(PresentAiBox.Items.Count > index)
                {
                    PresentAiBox.SelectedIndex = index;
                }
                else
                {
                    PresentAiBox.SelectedIndex = index - 1;
                }
            }
        }

        private void ClickGoButton(object sender, EventArgs e)
        {
            if(PresentAiBox.Items.Count < 2)
            {
                MessageBox.Show("Game needs at least two AIs");
                return;
            }
            if (RulesSelector.SelectedItem == null)
            {
                MessageBox.Show("You must select a rule set");
                return;
            }
            ShouldRunGame = true;
            var constants = Constants[(string)RulesSelector.SelectedItem];
            Game = new Game(constants, constants.TotalBots,
                PresentAiBox.Items.OfType<TeamInfoAttribute>()
                    .Select(x => ReflectionLoadedAis[x])
                    .Select(x => (TeamAi)Activator.CreateInstance(x))
                    .ToArray());
            this.Close();
        }
        
        public void HandleKey(object sender, KeyEventArgs args)
        {
            if(args.KeyCode == Keys.Right)
            {
                ClickAddButton(null, null);
                args.Handled = true;
            }
        }
    }
}
