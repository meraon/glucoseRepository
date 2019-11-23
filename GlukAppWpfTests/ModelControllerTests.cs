using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlukAppWpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlukAppWpf.Models;
using GlukLibrary;
using GlukLibrary.DbQuery;
using OxyPlot;
using OxyPlot.Axes;

namespace GlukAppWpf.Tests
{
    [TestClass()]
    public class ModelControllerTests
    {
        
        [TestMethod()]
        public void GlucoseCollectionChangedAddTest()
        {
            ModelProvider modelController = new ModelProvider();
            GlucoseItem value = new GlucoseItem(DateTime.Now, 9.0f);
            modelController.Glucoses.Add(value);
            DataPoint point = ModelProvider.ModelToDataPoint(value);

            Assert.IsTrue(modelController.GlucoseDataPoints.Contains(point));

            CheckGlucosesSorted(modelController);
        }

        

        [TestMethod()]
        public void GlucoseCollectionChangedRemoveTest()
        {
            ModelProvider modelController = new ModelProvider();
            Random r = new Random();
            int max = modelController.Glucoses.Count / 2;

            for (int i = 0; i < max; i++)
            {
                var item = modelController.Glucoses[r.Next(0, modelController.Glucoses.Count - 1)];
                modelController.Glucoses.Remove(item);
                
            }

            Assert.IsTrue( modelController.Glucoses.Count == modelController.GlucoseDataPoints.Count);
            CheckGlucosesSorted(modelController);

        }

        [TestMethod]
        public void GlucoseCollectionChangedReplaceTest()
        {
            ModelProvider modelController = new ModelProvider();
            var glucose = new GlucoseItem(DateTime.Now, 9.0f);

            modelController.Glucoses[0] = glucose;
            var point = ModelProvider.ModelToDataPoint(glucose);
            
            Assert.IsTrue(modelController.GlucoseDataPoints.Contains(point));

            CheckGlucosesSorted(modelController);

        }

        [TestMethod]
        public void GlucoseCollectionChangedResetTest()
        {
            ModelProvider modelController = new ModelProvider();
            modelController.Glucoses.Clear();
            Assert.IsTrue(modelController.GlucoseDataPoints.Count == 0);
        }

        [TestMethod()]
        public void InsulinCollectionChangedAddTest()
        {
            ModelProvider modelController = new ModelProvider();
            InsulinItem value = new InsulinItem(DateTime.Now, 9.0f, true);
            modelController.Insulins.Add(value);
            DataPoint point = ModelProvider.ModelToDataPoint(value);

            Assert.IsTrue(modelController.InsulinDataPoints.Contains(point));

            CheckInsulinsSorted(modelController);
        }



        [TestMethod()]
        public void InsulinCollectionChangedRemoveTest()
        {
            ModelProvider modelController = new ModelProvider();
            Random r = new Random();
            int max = modelController.Insulins.Count / 2;

            for (int i = 0; i < max; i++)
            {
                var item = modelController.Insulins[r.Next(0, modelController.Insulins.Count - 1)];
                modelController.Insulins.Remove(item);
            }

            Assert.IsTrue(modelController.Insulins.Count == modelController.InsulinDataPoints.Count);
            
            CheckInsulinsSorted(modelController);
        }

        [TestMethod]
        public void InsulinCollectionChangedReplaceTest()
        {
            ModelProvider modelController = new ModelProvider();
            var insulin = new InsulinItem(DateTime.Now, 9.0f, true);

            modelController.Insulins[0] = insulin;
            var point = ModelProvider.ModelToDataPoint(insulin);

            Assert.IsTrue(modelController.InsulinDataPoints.Contains(point));

            CheckInsulinsSorted(modelController);
        }

        [TestMethod]
        public void InsulinCollectionChangedResetTest()
        {
            ModelProvider modelController = new ModelProvider();
            modelController.Insulins.Clear();
            Assert.IsTrue(modelController.InsulinDataPoints.Count == 0);
        }

        [TestMethod]
        public void ModelToDataPointTest()
        {
            var dateTime = DateTime.Now;
            var glucose = new GlucoseItem(dateTime, 9.0f);
            var glucoseDatapoint = ModelProvider.ModelToDataPoint(glucose);
            Assert.AreEqual(glucoseDatapoint, new DataPoint(DateTimeAxis.ToDouble(dateTime), 9.0f));
        }

        private static void CheckGlucosesSorted(ModelProvider modelController)
        {
            var glucoses = modelController.Glucoses;
            glucoses.Sort(x => x.Date);
            int count = glucoses.Count;
            for (int i = 0; i < count; i++)
            {
                var point = ModelProvider.ModelToDataPoint(glucoses[i]);
                Assert.IsTrue(modelController.GlucoseDataPoints[i].Equals(point));
            }
        }

        private static void CheckInsulinsSorted(ModelProvider modelController)
        {
            var insulins = modelController.Insulins;
            insulins.Sort(x => x.Date);
            int count = insulins.Count;
            for (int i = 0; i < count; i++)
            {
                var point = ModelProvider.ModelToDataPoint(insulins[i]);
                Assert.IsTrue(modelController.InsulinDataPoints[i].Equals(point));
            }
        }


    }
}