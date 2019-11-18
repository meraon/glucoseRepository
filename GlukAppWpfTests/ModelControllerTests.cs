using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlukAppWpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlukLibrary;
using GlukLibrary.DbQuery;
using OxyPlot;

namespace GlukAppWpf.Tests
{
    [TestClass()]
    public class ModelControllerTests
    {
        
        [TestMethod()]
        public void GlucoseCollectionChangedAddTest()
        {
            ModelController modelController = new ModelController();
            Glucose value = new Glucose(HelperMethods.DateTimeToTimestamp(DateTime.Now), 9.0f);
            modelController.Glucoses.Add(value);
            DataPoint point = ModelController.ModelToDataPoint(value);

            Assert.IsTrue(modelController.GlucoseDataPoints.Contains(point));

            CheckGlucosesSorted(modelController);
        }

        

        [TestMethod()]
        public void GlucoseCollectionChangedRemoveTest()
        {
            ModelController modelController = new ModelController();
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
            ModelController modelController = new ModelController();
            var glucose = new Glucose(HelperMethods.DateTimeToTimestamp(DateTime.Now), 9.0f);

            modelController.Glucoses[0] = glucose;
            var point = ModelController.ModelToDataPoint(glucose);
            
            Assert.IsTrue(modelController.GlucoseDataPoints.Contains(point));

            CheckGlucosesSorted(modelController);

        }

        [TestMethod]
        public void GlucoseCollectionChangedResetTest()
        {
            ModelController modelController = new ModelController();
            modelController.Glucoses.Clear();
            Assert.IsTrue(modelController.GlucoseDataPoints.Count == 0);
        }

        [TestMethod()]
        public void InsulinCollectionChangedAddTest()
        {
            ModelController modelController = new ModelController();
            Insulin value = new Insulin(HelperMethods.DateTimeToTimestamp(DateTime.Now), 9.0f, true);
            modelController.Insulins.Add(value);
            DataPoint point = ModelController.ModelToDataPoint(value);

            Assert.IsTrue(modelController.InsulinDataPoints.Contains(point));

            CheckInsulinsSorted(modelController);
        }



        [TestMethod()]
        public void InsulinCollectionChangedRemoveTest()
        {
            ModelController modelController = new ModelController();
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
            ModelController modelController = new ModelController();
            var glucose = new Insulin(HelperMethods.DateTimeToTimestamp(DateTime.Now), 9.0f, true);

            modelController.Insulins[0] = glucose;
            var point = ModelController.ModelToDataPoint(glucose);

            Assert.IsTrue(modelController.InsulinDataPoints.Contains(point));

            CheckInsulinsSorted(modelController);
        }

        [TestMethod]
        public void InsulinCollectionChangedResetTest()
        {
            ModelController modelController = new ModelController();
            modelController.Insulins.Clear();
            Assert.IsTrue(modelController.InsulinDataPoints.Count == 0);
        }

        private static void CheckGlucosesSorted(ModelController modelController)
        {
            var glucoses = modelController.Glucoses;
            glucoses.Sort(x => x.getTimestamp());
            int count = glucoses.Count;
            for (int i = 0; i < count; i++)
            {
                var point = ModelController.ModelToDataPoint(glucoses[i]);
                Assert.IsTrue(modelController.GlucoseDataPoints[i].Equals(point));
            }
        }

        private static void CheckInsulinsSorted(ModelController modelController)
        {
            var insulins = modelController.Insulins;
            insulins.Sort(x => x.getTimestamp());
            int count = insulins.Count;
            for (int i = 0; i < count; i++)
            {
                var point = ModelController.ModelToDataPoint(insulins[i]);
                Assert.IsTrue(modelController.InsulinDataPoints[i].Equals(point));
            }
        }
    }
}