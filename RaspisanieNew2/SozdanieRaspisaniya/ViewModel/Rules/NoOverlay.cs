using Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SozdanieRaspisaniya.ViewModel.Rules
{
    public class NoOverlay : IRule
    {
        private Dictionary<int, DropInformation> groupInform = new Dictionary<int, DropInformation>();
        private Dictionary<(int, int), DropInformation> teacherInform = new Dictionary<(int, int), DropInformation>();
        private Dictionary<int, DropInformation> classroomInform = new Dictionary<int, DropInformation>();
        private string message = "";

        public void Check(ObservableCollection<ObservableCollection<DropItem>> filtered, ref List<string> listOfErrors)
        {
            foreach (var row in filtered)
            {
                foreach (var item in row)
                {
                    if (groupInform.ContainsKey(item.Item.Group.Single().CodeOfGroup))
                    {
                        message = "";
                    }
                }
            }
        }
    }
}
