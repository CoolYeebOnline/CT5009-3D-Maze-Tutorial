using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New UnoMaze", menuName = "Unomaze")]
public class UnoMaze : ScriptableObject
{
    private Cube[,,] MazeArray;
    private List<Cube> MazeList;
   // private Vector3 MazeSize;
    private int x;
    private int y;
    private int z;
    private GameObject MazeObject;
    private List<Cube> luckycubes;//!! make this private somehow?
    // Use this for initialization
    void Start () {
		
	}

    //public void Generate(Vector3 pos, Vector3 scale, int mode)//modes: 0 = plane, 1 = cube, 2 = sphere, 3 entire cube) 
    //{ }
    //public void Generate(Vector3 pos, Vector3 scale, int mode, int startx, int starty, int startz)//override for non-random start 
    //{ }
    public void Generate(Vector3 pos, Vector3 scale)//override for default mode (0)
    {

        x = Mathf.RoundToInt(scale.x);
        y = Mathf.RoundToInt(scale.y);
        z = Mathf.RoundToInt(scale.z); 
        MazeArray = new Cube[x, y, z];
        luckycubes = new List<Cube>();
        MazeList = new List<Cube>();
        
        SearchArray(0);//set outside layer to true
   //     Debug.Log("Start Prims:");
        PrimsAlgorithm();
        Build(pos, scale);

    }
    public void Delete()
    {
//        SearchArray(3);
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
        MazeBase.transform.localPosition = new Vector3(0,0,0);
        MazeBase.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        //     cube.transform.localScale = Parent.transform.localScale;

    }

    private Cube SearchArray(int mode)//searches through array via for loop, mode decides what to do with what is found
    {// mode 0 initiate carveability
        // mode 1 select random cube
        // mode 2 generate cubes
        int count = 0;
        for (int xi = 0; xi < x; xi++)
        {
            for (int yi = 0; yi < y; yi++)
            {
                for (int zi = 0; zi < z; zi++)
                {
                    switch (mode)
                    {
                        case 0:
                            if (zi == 0 || yi == 0 || xi == 0 || zi == z - 1 || yi == y - 1 || xi == x - 1)
                                {

                                    MazeArray[xi, yi, zi].SetCarveable(true);

                                MazeArray[xi, yi, zi].SetDeletable(false);
                                MazeArray[xi, yi, zi].SetPos(xi, yi, zi);
                                MazeArray[xi, yi, zi].SetWeight(Random.Range(1, 199));

                            }
                            else
                                {
                                    MazeArray[xi, yi, zi].SetCarveable(false);
                                    MazeArray[xi, yi, zi].SetDeletable(true);
                            }
                            
                            break;
                        case 1:
                            if (MazeArray[xi, yi, zi].GetCarveable() ==true)
                            {
                        
                                luckycubes.Add(MazeArray[xi, yi, zi]);
                                count++;
                            }
                            break;
                        case 2:
                            if (MazeArray[xi, yi, zi].GetCarveable()== true)
                            {
                                MazeArray[xi, yi, zi].Generate(MazeObject);
                            }
                            break;
                        case 3:
                            MazeArray[xi, yi, zi].Delete();
                            MazeArray[xi, yi, zi]= new Cube();
                            break;
                        default:
                            Debug.Log("Mode Input Error"); 
                            break;
                    }
                    
                    
                }
            }
        }

        if (mode == 1)
        {
            int rand = Random.Range(0, count);
    //        Debug.Log("Start Cube:" + luckycubes[rand].GetX() + " " + luckycubes[rand].GetY() + " " + luckycubes[rand].GetZ()); 
            return luckycubes[rand];
        }

        Cube badsearch = new Cube();
        badsearch.SetCarveable(false);
        return badsearch;
    }


    private void Cutout(Cube tCube)//call this function to cut a cube out of the maze to create a path
    {
        if (tCube.GetCarveable() == true)
        {
            MazeList.Add(MazeArray[tCube.GetX(), tCube.GetY(), tCube.GetZ()]);

            MazeArray[tCube.GetX(), tCube.GetY(), tCube.GetZ()].SetCarveable(false);
       //     Debug.Log(Cube.GetX()+ " "+ Cube.GetY() + " " + Cube.GetZ() + " " + "can carve: " + Cube.GetCarveable());
        }
    }

    private void PrimsAlgorithm()
    {

        Cutout(SearchArray(1));// set starting block
        bool isayso = true;
        Cube temp = new Cube();
        Cube TargetCube = new Cube();
        while (isayso)
        {

            TargetCube.SetWeight(999);
            foreach(Cube i in MazeList)
            {
                luckycubes.Clear();//we need to put neighbourcubes in this list!!

                temp = NeighbourCubes(i.GetX(), i.GetY(), i.GetZ(), 0);
      //          Debug.Log(TargetCube.GetWeight() + "= current best. Trying to beat:" + temp.GetWeight() + " for cube... " + i.GetX()+i.GetY()+i.GetZ() + temp.GetCarveable());
                if( MazeArray[temp.GetX() , temp.GetY() , temp.GetZ()].GetWeight() < TargetCube.GetWeight() && temp.GetCarveable()== true)
                {
                    TargetCube = temp;
                }

            }
        //    Debug.Log("One loop done!");
            if (TargetCube.GetWeight() == 999)
            {
                isayso = false;
            }
            else
            {
          //      Debug.Log(TargetCube.GetX() + TargetCube.GetY() +TargetCube.GetZ()+ " is being added");
                Cutout(TargetCube);
            }
        }
    }
    private Cube NeighbourCubes(int pointX, int pointY, int pointZ, int mode)//for loop for points 1 to -1 for x y and z of cube. Mode for function
    {
        
        //mode 0: initial check for adjacent neighbour squares
        //mode 1: checks that only one active neighbour is adjacent
        for (int xi = -1; xi <= 1; xi++)
        {

            for (int yi = -1; yi <= 1; yi++)
            {
                for (int zi = -1; zi <= 1; zi++)
                {
                    switch (mode)
                    {
                        case 0:

                            if (pointX + xi >= 0 && pointX + xi < x && pointY + yi >= 0 && pointY + yi < y && pointZ + zi >= 0 && pointZ + zi < z && !(pointX == 0 && pointY == 0 && pointZ == 0))
                            {//return true if value is within array and not a diagonal or itself, else false
                                if (MazeArray[pointX + xi, pointY + yi, pointZ + zi].GetCarveable())
                                {

                                    luckycubes.Clear();
             //                       Debug.Log("Checking.." + (pointX + xi) + (pointY + yi) + (pointZ + zi));
                                NeighbourCubes(pointX + xi, pointY + yi, pointZ + zi, 1);
                                if (luckycubes.Count == 1)
                                {
        //                            Debug.Log("goodsearch!!");
                                    luckycubes.Clear();
                                    return MazeArray[pointX + xi, pointY + yi, pointZ + zi];
                                }
                            }
                            }
                            break;
                        case 1:
                            if (!(Mathf.Pow(xi, 2) == 1 && Mathf.Pow(yi, 2) == 1 || Mathf.Pow(xi, 2) == 1 && Mathf.Pow(zi, 2) == 1 || Mathf.Pow(zi, 2) == 1 && Mathf.Pow(yi, 2) == 1) && !(xi == 0 && yi == 0 && zi == 0))
                            {//return true if value is within array and not a diagonal or itself, else false
                                if (pointX + xi >= 0 && pointX + xi < x && pointY + yi >= 0 && pointY + yi < y && pointZ + zi >= 0 && pointZ + zi < z && !(pointX == 0 && pointY == 0 && pointZ == 0))
                                {
                    //                 Debug.Log("Reversing..!" + (pointX+xi) + " "+ (pointY+yi)+ " "+ (pointZ+zi) + MazeArray[pointX + xi, pointY + yi, pointZ + zi].GetCarveable() + MazeArray[pointX + xi, pointY + yi, pointZ + zi].GetDeletable());
                                    if (MazeArray[pointX + xi, pointY + yi, pointZ + zi].GetCarveable()==false && MazeArray[pointX + xi, pointY + yi, pointZ + zi].GetDeletable() == false)
                                    {
                                        luckycubes.Add(MazeArray[pointX, pointY , pointZ ]);
                       //                 Debug.Log("Adding to new list..! " + luckycubes.Count);
                                    }
                                }
                            }
                            break;
                        default:
                            Debug.Log("Mode Input Error");
                            break;
                    }

                }

            }
        }
  
        Cube badsearch = new Cube();
        badsearch.SetWeight(999);
    //    Debug.Log("badsearch!!");
        return badsearch;
    }

    private struct Cube
    {
        private float weight;//turn this entire script into a struct?   
        private bool carveable;
        private bool toDelete;
        private GameObject cube;
        private int x;
        private int y;
        private int z;
        private GameObject prefab;

        

        public void Generate(GameObject Parent) //instantiate cube
        {
            if (prefab == null)
            {
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            else
            {
                cube = Instantiate(prefab, new Vector3((x / 2) - x, (y / 2) - y, (z / 2) - z), Quaternion.identity);
            }

            cube.transform.parent = Parent.transform;
            cube.transform.localPosition = new Vector3((x + 0.5f-(Parent.transform.localScale.x / 2))/ Parent.transform.localScale.x, (y + 0.5f - (Parent.transform.localScale.y / 2))/ Parent.transform.localScale.y, (z + 0.5f - (Parent.transform.localScale.z / 2))/ Parent.transform.localScale.z);
       //     cube.transform.localScale = Parent.transform.localScale;
       //     cube.name = ( GetWeight()+ "g Block:" + x + " " + y + " " + z );
            if(carveable)
            {
                cube.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        public void Delete() //deletes cube
        {
            Destroy(cube);
        }
        public float GetWeight() { return weight; }
        public void SetWeight(float input) { weight = input; }
        public bool GetCarveable() { return carveable; }
        public void SetCarveable(bool NewCarveable) { carveable = NewCarveable; }
        public bool GetDeletable() { return toDelete; }
        public void SetDeletable(bool NewBool) { toDelete = NewBool; }



        public int GetX() { return x; }
        public int GetY() { return y; }
        public int GetZ() { return z; }
        public void SetPos(int xpos, int ypos, int zpos)
        {
            x = xpos;
            y = ypos;
            z = zpos;
        }
    }
}
