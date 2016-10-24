using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using InvadersWindowsStoreApp.Model;
using Windows.Foundation;
using System.Threading.Tasks;

namespace UnitTestInvaders
{
    [TestClass]
    public class UnitTestingInvadersModel
    {
        [TestMethod]
        public void TestMoveReturnsNewLocation()
        {
            Player player = new Player();
            Direction direction = Direction.Right;
            Point oldLocation = player.Location;
            player.Move(direction);
            Point newLocation = player.Location;

            Assert.AreNotEqual(oldLocation, newLocation, "Should not be the same. Player should move to a new Location.");          
        }

        [TestMethod]
        public void TestAddStarAddsStar()
        {
            InvaderModel invaderModel = new InvaderModel();
            int oldCountOfStars = invaderModel.Stars.Count;
            invaderModel.AddAStar();
            int newCountOfStars = invaderModel.Stars.Count;

            Assert.AreNotEqual(oldCountOfStars, newCountOfStars, "Should not be the same. Should contain a new Point element in Stars.");
        }
    }
}
