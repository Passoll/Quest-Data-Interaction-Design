using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Oculus.Interaction;
using Unity.XR.CoreUtils;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


namespace DxRextention
{
    /*
     * Written by: Ayse Zhong, 2023.7
     *
     * @SliderRange
     * brief: This is the range class that need to evaluate and store the range data, all the data valid is from 0 to 1;
     * This can maintain a list of ranges that separate with each other.
     *
     * @Method
     * - Contain()
     * 1. Test a number inside
     * 
     * - AddRange()
     * 1. Add the new range to find the intersection
     * 
     * - Invert()
     * 1. When change the state or invert all the range
     * 
     */
    public class SliderRange
    {
        //if end > start, then it contain the inverted number
        private List<(float, float)> ranges;

        public SliderRange()
        {
            ranges = new List<(float, float)> { (float.NegativeInfinity, float.PositiveInfinity) };
        }
        public SliderRange(float rangemark,bool state)
        {
            if (state)
            {
                ranges = new List<(float, float)> { (rangemark, float.PositiveInfinity) };
            }
            else
            {
                ranges = new List<(float, float)> { (float.NegativeInfinity, rangemark) };
            }
        }

        public void clearRange()
        {
            ranges.Clear();
            ranges= new List<(float, float)> { (float.NegativeInfinity, float.PositiveInfinity) };
        }

        public float GetItem1()
        {
            return ranges[0].Item1;
        }
        public float GetItem2()
        {
            return ranges[0].Item2;
        }
        public bool Contains(float value)
        {
            foreach (var range in ranges)
            {
                if (value >= range.Item1 && value <= range.Item2)
                {
                    return true;
                }
            }
            return false;
        }
        //Only call when adding to range, but can not handle situation that update range
        public void AddRange((float, float) newRange)
        {
            var intersections = new List<(float, float)>();
            foreach (var range in ranges)
            {
                var intersection = IntersectRanges(range, newRange);
                if (intersection != null)
                {
                    intersections.Add(intersection.Value);
                }
            }
            ranges = intersections;
        }
        
        public void Invert()
        {
            var invertedRanges = new List<(float, float)>();
            float prevEnd = float.NegativeInfinity;
            foreach (var range in ranges.OrderBy(r => r.Item1))
            {
                invertedRanges.Add((prevEnd, range.Item1));
                prevEnd = range.Item2;
            }
            invertedRanges.Add((prevEnd, float.PositiveInfinity));
            ranges = invertedRanges;
        }

        private static (float, float)? IntersectRanges((float, float) range1, (float, float) range2)
        {
            float start = Math.Max(range1.Item1, range2.Item1);
            float end = Math.Min(range1.Item2, range2.Item2);
            if (start <= end)
            {
                return (start, end);
            }
            return null;
        }
    }
    
    /*
     * Written by: Ayse Zhong, 2023.7
     *
     * @SliderManager
     * brief: Control all the slider and get the range
     *
     * @Method
     * - Addslider()
     * 1. When a slider is successfully initialized, this will add to the slider list;
     * 
     * - RemoveSlider()
     * - ClearSLider
     * 
     * - UpdateInstance()
     * 1. Call when slider have change. To disable the unselected interatable
     * 
     */

    public class SliderManager : MonoBehaviour
    {
        [Tooltip("Max slider on one axis")]
        public int maxslider = 5;
        
        public Dictionary<GameObject, SliderRange> XInstances = new Dictionary<GameObject, SliderRange>();
        public Dictionary<GameObject, SliderRange> YInstances = new Dictionary<GameObject, SliderRange>();
        public Dictionary<GameObject, SliderRange> ZInstances = new Dictionary<GameObject, SliderRange>();
        private List<SliderRange> XYZranges = new List<SliderRange>();
        
        public DxRtransformcontroller _DxRtransformcontroller;
        public DxR.Vis DxRManager;
        //Store the Mark iterable and mark
        private Dictionary<GameObject, Transform> _DxRmarkinstance = new Dictionary<GameObject, Transform>();
        public Transform anchor;

        private void Start()
        {
            foreach (var instance in DxRManager.markInstances)
            {
                _DxRmarkinstance.Add(instance, instance.transform.Find("Interactable"));
            }

            XYZranges.Add(new SliderRange());
            XYZranges.Add(new SliderRange());
            XYZranges.Add(new SliderRange());

        }

        //TOD1 : call every time switch the data
        public void InitializeMarkInstance()
        {
            foreach (var instance in DxRManager.markInstances)
            {
                _DxRmarkinstance.Add(instance, instance.transform.Find("Interactable"));
            }
        }

        public bool Addslider(SlidingBlock.TAxis axis, GameObject newslider, SliderRange newrange)
        {
            switch (axis)
            {
                case SlidingBlock.TAxis.X:
                    if (XInstances.Count < maxslider)
                    {
                        XInstances[newslider] = newrange;
                        return true;
                    }
                    break;
                case SlidingBlock.TAxis.Y:
                    if (YInstances.Count < maxslider)
                    {
                        YInstances[newslider] = newrange;
                        return true;
                    }
                    break;
                case SlidingBlock.TAxis.Z:
                    if (ZInstances.Count < maxslider)
                    {
                        ZInstances[newslider] = newrange;
                        return true;
                    }
                    break;
            }
            return false;
        }

        //Clear instance in the block and Update the range
        public void RemoveSliderCheck(SlidingBlock.TAxis axis)
        {
            switch (axis)
            {
                case SlidingBlock.TAxis.X:
                    RemoveNullinDic(ref XInstances);
                    XYZranges[0].clearRange();
                    foreach (var kvp in XInstances)
                    {
                        XYZranges[0].AddRange((kvp.Value.GetItem1(), kvp.Value.GetItem2()));
                    }
                    break;
                case SlidingBlock.TAxis.Y:
                    RemoveNullinDic(ref YInstances);
                    XYZranges[1].clearRange();
                    foreach (var kvp in YInstances)
                    {
                        XYZranges[1].AddRange((kvp.Value.GetItem1(), kvp.Value.GetItem2()));
                    }
                    break;
                case SlidingBlock.TAxis.Z:
                    RemoveNullinDic(ref ZInstances);
                    XYZranges[2].clearRange();
                    foreach (var kvp in ZInstances)
                    {
                        XYZranges[2].AddRange((kvp.Value.GetItem1(), kvp.Value.GetItem2()));
                    }
                    break;
            }
        }

        
        public bool Clearslider(SlidingBlock.TAxis axis)
        {
            //TOD1 Clear all slider
            return true;
        }
        
        
    
        //Call When change the Current instance
        public void UpdateCurrentRange(GameObject instance, SliderRange temprange, SlidingBlock.TAxis axis)
        {
            switch (axis)
            {
                case SlidingBlock.TAxis.X:
                    //UpdateTempRange
                    XYZranges[0].clearRange();
                    XInstances[instance] = temprange;
                    foreach (var kvp in XInstances)
                    {
                        XYZranges[0].AddRange((kvp.Value.GetItem1(), kvp.Value.GetItem2()));
                    }
                    XYZranges[0].AddRange((temprange.GetItem1(), temprange.GetItem2()));
                    UpdateMarkInstance();
                    XYZranges[0].clearRange();
                    
                    break;
                case SlidingBlock.TAxis.Y:
                    
                    XYZranges[1].clearRange();
                    YInstances[instance] = temprange;
                    foreach (var kvp in YInstances)
                    {
                        XYZranges[1].AddRange((kvp.Value.GetItem1(), kvp.Value.GetItem2()));
                    }
                    XYZranges[1].AddRange((temprange.GetItem1(), temprange.GetItem2()));
                    UpdateMarkInstance();
                    XYZranges[1].clearRange();
                    
                    break;
                case SlidingBlock.TAxis.Z:
                    
                    XYZranges[2].clearRange();
                    ZInstances[instance] = temprange;
                    foreach (var kvp in ZInstances)
                    {
                        XYZranges[2].AddRange((kvp.Value.GetItem1(), kvp.Value.GetItem2()));
                    }
                    XYZranges[2].AddRange((temprange.GetItem1(), temprange.GetItem2()));
                    UpdateMarkInstance();
                    XYZranges[2].clearRange();
                    
                    break;
            }
        }
        

        //check in every move
        public void UpdateMarkInstance()
        {
            foreach (var instance in _DxRmarkinstance)
            {
                if (!CheckinRange(instance.Key.transform))
                {
                    instance.Value.gameObject.SetActive(false);
                }
                else
                {
                    instance.Value.gameObject.SetActive(true);
                }
            }
        }

        private bool CheckinRange(Transform instance)
        {
            Vector3 instance_vec = instance.position - anchor.position;
            Vector3 boundSize = _DxRtransformcontroller.GetBoundSize();
            Vector3 rate = new Vector3(instance_vec.x / boundSize.x,
                instance_vec.y / boundSize.y,
                instance_vec.z / boundSize.z);
            
            bool result = XYZranges[0].Contains(rate.x) && 
                          XYZranges[1].Contains(rate.y) && 
                          XYZranges[2].Contains(rate.z);
            
            return result;
        }
        
        private void RemoveNullinDic(ref Dictionary<GameObject, SliderRange> dic)
        {
            List<GameObject> keystoremove = new List<GameObject>();
            if (dic.Count > 0)
            {
                foreach (var kvp in dic)
                {
                    if (kvp.Key == null)
                    {
                        keystoremove.Add(kvp.Key);
                    }
                }
                foreach (GameObject key in keystoremove)
                {
                    dic.Remove(key);
                }
            }
            
        }

    }
}

