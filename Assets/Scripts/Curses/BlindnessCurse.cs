using System.Collections;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

namespace Curses
{
    public class BlindnessCurse : Curse
    {
        private readonly List<Cloud> clouds;
        
        public BlindnessCurse(Field field, int count)
        {
            clouds = new List<Cloud>();
            
            for (var i = 0; i < count; i++)
            {
                var cloud = field.CreateCloud();
                cloud.transform.position = Vector3.zero.RandomOffset(2.5f);
                clouds.Add(cloud);
            }
        }
        public override IEnumerator Apply(Field field)
        {
            clouds.ForEach(MoveCloud);
            yield return new WaitForSeconds(0.5f);
        }

        private static void MoveCloud(Cloud cloud)
        {
            Tweener.MoveToQuad(cloud.transform, Vector3.zero.RandomOffset(2.5f), Random.Range(2f, 5f));
        }

        public override string GetTutorial()
        {
            return "There seems to be some (visual interference) in this stage...";
        }
    }
}