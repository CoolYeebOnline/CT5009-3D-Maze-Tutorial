using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Maze", menuName = "PrimsMaze")]
public class MazeGenerator : ScriptableObject
{
    private Cube[,,] MazeArray;
    private List<Cube> MazeList;
    private int x, y, z;
    private GameObject MazeObject;
    private List<Cube> Luckycubes;

    public void Generate(Vector3 pos, Vector3 scale)
    {
        x = Mathf.RoundToInt(scale.x);
        y = Mathf.RoundToInt(scale.y);
        z = Mathf.RoundToInt(scale.z);

        MazeArray = new Cube[x, y, z];
        Luckycubes = new List<Cube>();
        MazeList = new List<Cube>();

        SearchArray(0);
        PrimsAlgorithm();
        Build(pos, scale);
    }

    public void Delete()
    {
        Destroy(MazeObject);
    }

    public void Build(Vector3 pos, Vector3 scale)
    {
        MazeObject = new GameObject();
        MazeObject.transform.position = pos;
        MazeObject.transform.localScale = scale;

        SearchArray(2);

        GameObject MazeBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MazeBase.transform.parent = MazeObject.transform;
        MazeBase.transform.localPosition = new Vector3(0, 0, 0);
        MazeBase.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }

    private Cube SearchArray(int mode)
    {
        int count = 0;
        for(int xi = 0; xi<x; xi++)
        {
            for(int yi = 0; yi < y; yi++)
            {
                for(int zi = 0; zi < z; z++)
                {
                    switch (mode)
                    {
                        case 0:
                            if(zi == 0 || yi == 0 || xi == 0 || zi == z-1 | yi== y -1 | xi == x - 1)
                            {
                                MazeArray[xi, yi, zi].SetCarveable(true);
                                MazeArray[xi, yi, zi].SetDelete(false);
                                MazeArray[xi, yi, zi].SetPos(xi, yi, zi);
                                MazeArray[xi, yi, zi].SetWeight(Random.Range(1, 199));
                            }
                            else 
                            {
                                MazeArray[xi, yi, zi].SetCarveable(false);
                                MazeArray[xi, yi, zi].SetDelete(true);                           
                            }
                        break;
                        case 1:
                            if(MazeArray[xi,yi,zi].GetCarveable() == true)
                            {
                                Luckycubes.Add(MazeArray[xi, yi, zi]);
                                count++;
                            }
                            break;
                        case 2:
                            if(MazeArray[xi,yi,zi].GetCarveable() == true)
                            {
                                MazeArray[xi, yi, zi].Generate(MazeObject);
                            }

                            break;
                        default:
                            Debug.Log("Input Error");
                            break;
                    }
                }
            }
        }

        if(mode==1)
        {
            int rand = Random.Range(0, count);
        }
        Cube badsearch = new Cube();
        badsearch.SetCarveable(false);
        return badsearch;
    }

    private void CutOut(Cube tCube)
    {
        if(tCube.GetCarveable())
        {
            MazeList.Add(MazeArray[tCube.GetX(), tCube.GetY(), tCube.GetX()]);
            MazeArray[tCube.GetX(), tCube.GetY(), tCube.GetX()].SetCarveable(false);
        }
    }

    private void PrimsAlgorithm()
    {
        CutOut(SearchArray(1));
        bool isayso = true;
        Cube temp = new Cube();
        Cube TargetCube = new Cube();

        while (isayso)
        {
            TargetCube.SetWeight(999);
            foreach(Cube i in MazeList)
            {
                Luckycubes.Clear();
                temp = NeighbourCubes(i.GetX(), i.GetY(), i.GetZ(), 0);
                if(MazeArray[temp.GetX(), temp.GetY(), temp.GetZ()].GetWeight() < TargetCube.GetWeight())
                {
                    TargetCube = temp;
                }
            }

            if(TargetCube.GetWeight() == 999)
            {
                isayso = false;
            }
            else
            {
                CutOut(TargetCube);
            }
        }
    }

    private Cube NeighbourCubes(int pointx, int pointy,int pointz, int mode)
    {
        for(int xi = -1; xi<=1; xi++)
        {
            for(int yi = -1; yi<= 1; yi++ )
            {
                for(int zi = -1; zi<= 1; zi++)
                {
                    switch (mode)
                    {
                        case 0:
                            if(pointx + xi >=0 && pointx+xi < x && pointy + yi >= 0 && pointy + yi < y && pointz + zi >= 0 && pointz + zi < z)
                            {
                                Luckycubes.Clear();
                                NeighbourCubes(pointx + xi, pointy + yi, pointz + zi, 1);
                                if(Luckycubes.Count ==1)
                                {
                                    return MazeArray[pointx + xi, pointy + yi, pointz + zi];
                                }
                            }

                            break;
                        case 1:
                            if(!(Mathf.Pow(xi,2) ==1 && Mathf.Pow(yi,2)==1) || !(Mathf.Pow(xi,2) == 1 && Mathf.Pow(zi,2) ==1) || !(Mathf.Pow(yi,2)==1 && Mathf.Pow(zi,2) == 1) &&!(xi==0 && yi==0 && zi ==0))
                            {
                                if(pointx + xi >= 0 && pointx + xi < x && pointy + yi >= 0 && pointy + yi < y && pointz + zi >= 0 && pointz + zi < z)
                                {
                                    if(MazeArray[pointx+xi, pointy+yi, pointz+zi].GetCarveable() == false && MazeArray[pointx+xi, pointy+yi,pointz+zi].GetDelete()== false)
                                    {
                                        Luckycubes.Add(MazeArray[pointx, pointy, pointz]);
                                    }
                                }
                            }
                            break;
                        default:
                            Debug.Log("Input Error");
                            break;
                    }
                }
            }
        }
        Cube badsearch = new Cube();
        badsearch.SetWeight(999);
        return badsearch;
    }

    private struct Cube
    {
        public float weight;
        private bool carveable;
        private bool toDelete;
        private GameObject cube;
        private int x;
        private int y;
        private int z;
        private GameObject prefab;
        public void Generate(GameObject Parent)
        {
            if(prefab == null)
            {
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            else
            {
                cube = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            }
            cube.transform.parent = Parent.transform;
            cube.transform.localPosition = new Vector3((x+ 0.5f - (Parent.transform.localScale.x / 2))/ Parent.transform.localScale.x, (y + 0.5f - (Parent.transform.localScale.y / 2)) / Parent.transform.localScale.y, (z + 0.5f - Parent.transform.localScale.z /2)) / Parent.transform.localScale.z;

            if(carveable)
            {
                cube.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        public float GetWeight() { return weight; }
        public void SetWeight(float input) { weight = input; }
        public bool GetCarveable() { return carveable; }
        public void SetCarveable(bool input) { carveable = input; }
        public bool GetDelete() { return toDelete; }
        public void SetDelete(bool input) { toDelete = input; }
        public void SetPos(int xpos, int ypos, int zpos)
        {
            x = xpos;
            y = ypos;
            z = zpos;
        }
        public int GetX() { return x; }
        public int GetY() { return y; }
        public int GetZ() { return z; }
    }
    
}
