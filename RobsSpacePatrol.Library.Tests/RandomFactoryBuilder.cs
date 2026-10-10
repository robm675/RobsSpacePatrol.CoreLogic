
using System;
using RobsSpacePatrol.Library.eNums;
using RobsSpacePatrol.Library.Factories.Interfaces;
using RobsSpacePatrol.Library.Models;

namespace RobsSpacePatrol.CoreLogic.Tests;

public class RandomFactoryBuilder
{
    int EnemyChance = 0;
    int StarBaseChance = 0;
    int StarQuantity = 0;
    int EnemyShieldLevel = 0;
    List<Coord> EnemyLocations = [];
    Coord StarBaseLocation = new(-1, -1);
    List<Coord> StarLocations = [];
    Coord StarshipSector = new(-1, -1);
    Coord StarshipQuadrant = new(-1, -1);
    int EnemyFiredAmount = 0;
    int EnemyMoveChance = 0;
    int DamagedDevice = -1;
    int DamagedDeviceAmount = 0;
    int DamagedDeviceChance = 0;
    int StarDestroyedChance = 0;
    int LaserMissChance = 0;
    int StartingMissionTime = 2400;

    public RandomFactoryBuilder setEnemyChance(int value)
    {
        EnemyChance = value;
        return this;
    }
    public RandomFactoryBuilder setStarBaseChance(int value)
    {
        StarBaseChance = value;
        return this;
    }
    public RandomFactoryBuilder setStarQuantity(int value)
    {
        StarQuantity = value;
        return this;
    }
    public RandomFactoryBuilder setEnemyShieldLevel(int value)
    {
        EnemyShieldLevel = value;
        return this;
    }
    public RandomFactoryBuilder setEnemyFiredAmount(int value)
    {
        EnemyFiredAmount = value;
        return this;
    }
    public RandomFactoryBuilder setEnemyMoveChance(int value)
    {
        EnemyMoveChance = value;
        return this;
    }
    public RandomFactoryBuilder setDamagedDevice(int value)
    {
        DamagedDevice = value;
        return this;
    }
    public RandomFactoryBuilder setDamagedDeviceAmount(int value)
    {
        DamagedDeviceAmount = value;
        return this;
    }
    public RandomFactoryBuilder setDamagedDeviceChance(int value)
    {
        DamagedDeviceChance = value;
        return this;
    }
    public RandomFactoryBuilder setStarDestroyedChance(int value)
    {
        StarDestroyedChance = value;
        return this;
    }
    public RandomFactoryBuilder setLaserMissChance(int value)
    {
        LaserMissChance = value;
        return this;
    }
    public RandomFactoryBuilder setStartingMissionTime(int value)
    {
        StartingMissionTime = value;
        return this;
    }
    public RandomFactoryBuilder clearEnemyLocations()
    {
        EnemyLocations.Clear();
        return this;
    }
    public RandomFactoryBuilder clearStarLocations()
    {
        StarLocations.Clear();
        return this;
    }
    public RandomFactoryBuilder addEnemyLocation(Coord value)
    {
        EnemyLocations.Add(value);
        return this;
    }
    public RandomFactoryBuilder addStarLocation(Coord value)
    {
        StarLocations.Add(value);
        return this;
    }
    public RandomFactoryBuilder setStarbaseLocation(Coord value)
    {
        StarBaseLocation = value;
        return this;
    }
    public RandomFactoryBuilder setStarshipSector(Coord value)
    {
        StarshipSector = value;
        return this;
    }
    public RandomFactoryBuilder setStarshipQuadrant(Coord value)
    {
        StarshipQuadrant = value;
        return this;
    }
    public IRandomFactory GetFactories()
    {
        var mockF = new Mock<IRandomFactory>();

        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.EnemyChance)))).Returns(EnemyChance);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.StarQuantity)))).Returns(StarQuantity);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.EnemyShieldLevel)))).Returns(EnemyShieldLevel);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.EnemyFiredAmount)))).Returns(EnemyFiredAmount);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.EnemyMoveChance)))).Returns(EnemyMoveChance);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.DamagedDevice)))).Returns(DamagedDevice);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.DamagedDeviceAmount)))).Returns(DamagedDeviceAmount);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.DamagedDeviceChance)))).Returns(DamagedDeviceChance);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.StarDestroyedChance)))).Returns(StarDestroyedChance);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.LaserMissChance)))).Returns(LaserMissChance);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.StartingMissionTime)))).Returns(StartingMissionTime);
        mockF.Setup(x => x.GetRandomInteger(It.Is<eRandomIntType>
                (y => y.Equals(eRandomIntType.StarBaseChance)))).Returns(StarBaseChance);

        mockF.Setup(x => x.GetRandomCoord(It.Is<eRandomCoordType>
                (y => y.Equals(eRandomCoordType.StarBaseLocation)))).Returns(StarBaseLocation);
        mockF.Setup(x => x.GetRandomCoord(It.Is<eRandomCoordType>
                (y => y.Equals(eRandomCoordType.StarshipSector)))).Returns(StarshipSector);
        mockF.Setup(x => x.GetRandomCoord(It.Is<eRandomCoordType>
                (y => y.Equals(eRandomCoordType.StarshipQuadrant)))).Returns(StarshipQuadrant);

        mockF.Setup(x => x.GetRandomCoord(It.Is<eRandomCoordType>
                (y => y.Equals(eRandomCoordType.StarshipQuadrant)))).Returns(StarshipQuadrant);

        if (EnemyLocations.Count > 0)
        {
            Guid GNID = Guid.NewGuid();
            mockF.Setup(x => x.GetRandomCoord(It.Is<eRandomCoordType>
                (y => y.Equals(eRandomCoordType.EnemyLocation)))).Returns(() => getNextCoord(EnemyLocations, GNID));
        }
        if (StarLocations.Count > 0)
        {
            Guid GNID = Guid.NewGuid();
            mockF.Setup(x => x.GetRandomCoord(It.Is<eRandomCoordType>
                (y => y.Equals(eRandomCoordType.StarLocation)))).Returns(() => getNextCoord(StarLocations, GNID));
        }

        return mockF.Object;
    }
    //int enemyIndex = -1;
    //Coord getNextEnemyCoord()
    //{
    //    enemyIndex++;
    //    if (enemyIndex > EnemyLocations.Count -1 ) enemyIndex = 0;
    //    return EnemyLocations[enemyIndex];
    //}




    Dictionary<Guid, int> dIDX_Coord = new Dictionary<Guid, int>();
    Coord getNextCoord(List<Coord> ReturnValues, Guid ID)
    {
        if (!dIDX_Coord.ContainsKey(ID))
        {
            dIDX_Coord.Add(ID, -1);
        }

        int tempIDX = dIDX_Coord[ID];
        tempIDX += 1;
        dIDX_Coord[ID] = tempIDX;

        if (tempIDX > ReturnValues.Count - 1) { dIDX_Coord[ID] = 0; }
        return ReturnValues[dIDX_Coord[ID]];
    }
}
