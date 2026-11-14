using System.Collections.Generic;
using UnityEngine;

namespace Afterlight
{
    public static class HarborDistrictBuilder
    {
        private static Material sand;
        private static Material paving;
        private static Material road;
        private static Material asphalt;
        private static Material whitePaint;
        private static Material stone;
        private static Material plaster;
        private static Material paleBlue;
        private static Material coral;
        private static Material terracotta;
        private static Material slate;
        private static Material timber;
        private static Material darkMetal;
        private static Material glass;
        private static Material foliage;
        private static Material palmLeaf;
        private static Material water;
        private static Material gold;
        private static Material paper;
        private static Material signalGlow;

        public static void Build(Transform root, MissionManager mission, GameUI ui)
        {
            CreateMaterials();
            CreateIsland(root);
            CreateStreets(root);
            CreateMarketBuildings(root);
            CreateHarbour(root);
            CreateVegetation(root);
            CreateStreetFurniture(root);
            CreateLighthouse(root);
            CreateInvestigationSites(root, mission);
            CreateMara(root, ui);
        }

        private static void CreateMaterials()
        {
            sand = MaterialFactory.Create("Warm coastal sand", new Color(0.70f, 0.58f, 0.40f), 0f, 0.08f);
            paving = MaterialFactory.Create("Weathered limestone paving", new Color(0.48f, 0.55f, 0.54f), 0f, 0.12f);
            road = MaterialFactory.Create("Old harbour asphalt", new Color(0.18f, 0.21f, 0.22f), 0f, 0.1f);
            asphalt = MaterialFactory.Create("Road edge stone", new Color(0.30f, 0.34f, 0.34f), 0f, 0.12f);
            whitePaint = MaterialFactory.Create("Faded road paint", new Color(0.81f, 0.77f, 0.64f), 0f, 0.05f);
            stone = MaterialFactory.Create("Salt-stained stone", new Color(0.42f, 0.43f, 0.39f), 0f, 0.18f);
            plaster = MaterialFactory.Create("Sun-faded ochre plaster", new Color(0.69f, 0.49f, 0.32f), 0f, 0.08f);
            paleBlue = MaterialFactory.Create("Harbour blue plaster", new Color(0.30f, 0.43f, 0.49f), 0f, 0.1f);
            coral = MaterialFactory.Create("Washed coral plaster", new Color(0.58f, 0.34f, 0.28f), 0f, 0.12f);
            terracotta = MaterialFactory.Create("Clay roof tiles", new Color(0.42f, 0.17f, 0.12f), 0f, 0.22f);
            slate = MaterialFactory.Create("Aged slate roof", new Color(0.16f, 0.21f, 0.24f), 0f, 0.25f);
            timber = MaterialFactory.Create("Salt-weathered timber", new Color(0.31f, 0.21f, 0.14f), 0f, 0.16f);
            darkMetal = MaterialFactory.Create("Blackened harbour metal", new Color(0.09f, 0.12f, 0.13f), 0.65f, 0.38f);
            glass = MaterialFactory.Create("Deep blue window glass", new Color(0.12f, 0.22f, 0.27f), 0.22f, 0.8f);
            foliage = MaterialFactory.Create("Coastal green foliage", new Color(0.17f, 0.31f, 0.21f), 0f, 0.1f);
            palmLeaf = MaterialFactory.Create("Palm frond", new Color(0.25f, 0.39f, 0.24f), 0f, 0.08f);
            water = MaterialFactory.Create("Harbour water", new Color(0.07f, 0.25f, 0.32f), 0.32f, 0.82f);
            gold = MaterialFactory.Create("Old brass", new Color(0.55f, 0.38f, 0.16f), 0.55f, 0.5f);
            paper = MaterialFactory.Create("Faded paper", new Color(0.78f, 0.69f, 0.49f), 0f, 0.04f);
            signalGlow = MaterialFactory.Emissive("Signal beacon", new Color(0.96f, 0.49f, 0.18f), 1.6f);
        }

        private static void CreateIsland(Transform root)
        {
            var sea = MeshFactory.MeshObject("Open water", CreateHorizontalQuad(180f, 180f, -0.28f),
                water, root);
            sea.transform.localPosition = new Vector3(0, -0.15f, 0);

            var island = new GameObject("Nambara island / natural coastline");
            island.transform.SetParent(root, false);
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            const int rings = 9;
            const int segments = 72;
            vertices.Add(new Vector3(0, 0.18f, 0));

            for (int ring = 1; ring <= rings; ring++)
            {
                float t = (float)ring / rings;
                for (int i = 0; i < segments; i++)
                {
                    float angle = i * Mathf.PI * 2f / segments;
                    float irregular = 1f + 0.045f * Mathf.Sin(angle * 3f + 0.8f)
                        + 0.028f * Mathf.Sin(angle * 7f - 1.1f);
                    float radiusX = 28f * t * irregular;
                    float radiusZ = 24f * t * irregular;
                    float x = Mathf.Cos(angle) * radiusX;
                    float z = Mathf.Sin(angle) * radiusZ;
                    float height = 0.18f + 0.10f * Mathf.Sin(x * 0.19f) * Mathf.Cos(z * 0.16f);
                    if (t > 0.82f) height -= Mathf.Pow((t - 0.82f) / 0.18f, 1.5f) * 0.72f;
                    vertices.Add(new Vector3(x, height, z));
                }
            }

            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;
                triangles.Add(0); triangles.Add(1 + next); triangles.Add(1 + i);
            }

            for (int ring = 1; ring < rings; ring++)
            {
                int innerStart = 1 + (ring - 1) * segments;
                int outerStart = 1 + ring * segments;
                for (int i = 0; i < segments; i++)
                {
                    int next = (i + 1) % segments;
                    int a = innerStart + i;
                    int b = innerStart + next;
                    int c = outerStart + i;
                    int d = outerStart + next;
                    triangles.Add(a); triangles.Add(b); triangles.Add(c);
                    triangles.Add(b); triangles.Add(d); triangles.Add(c);
                }
            }

            MeshFactory.MeshObject("Rolling island terrain", MeshFactory.BuildMesh("Coastal terrain", vertices, triangles),
                sand, island.transform, true);

            var beach = MeshFactory.MeshObject("Limestone shoreline", CreateShoreRing(25.0f, 28.1f, 23f, 26f),
                paving, island.transform);
            beach.transform.localPosition = new Vector3(0, -0.02f, 0);

            // Rocky outcrops break up the otherwise smooth edge of the island.
            CreateRock(root, new Vector3(-20f, 0.02f, -11f), 2.1f, stone);
            CreateRock(root, new Vector3(21f, -0.03f, -17f), 2.8f, stone);
            CreateRock(root, new Vector3(-17f, -0.03f, 17f), 2.4f, stone);
        }

        private static Mesh CreateHorizontalQuad(float width, float depth, float y)
        {
            var v = new List<Vector3>();
            var t = new List<int>();
            MeshFactory.AddQuad(v, t,
                new Vector3(-width * 0.5f, y, -depth * 0.5f),
                new Vector3(-width * 0.5f, y, depth * 0.5f),
                new Vector3(width * 0.5f, y, depth * 0.5f),
                new Vector3(width * 0.5f, y, -depth * 0.5f));
            return MeshFactory.BuildMesh("Horizontal surface", v, t);
        }

        private static Mesh CreateShoreRing(float innerX, float outerX, float innerZ, float outerZ)
        {
            var v = new List<Vector3>();
            var t = new List<int>();
            const int segments = 72;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                float irregular = 1f + 0.035f * Mathf.Sin(angle * 3f);
                v.Add(new Vector3(Mathf.Cos(angle) * innerX * irregular, 0.12f, Mathf.Sin(angle) * innerZ * irregular));
                v.Add(new Vector3(Mathf.Cos(angle) * outerX * irregular, -0.16f, Mathf.Sin(angle) * outerZ * irregular));
            }
            for (int i = 0; i < segments; i++)
            {
                int a = i * 2;
                t.Add(a); t.Add(a + 2); t.Add(a + 1);
                t.Add(a + 2); t.Add(a + 3); t.Add(a + 1);
            }
            return MeshFactory.BuildMesh("Shoreline ring", v, t);
        }

        private static void CreateRock(Transform parent, Vector3 position, float size, Material material)
        {
            var rock = MeshFactory.MeshObject("Weathered coastal rock",
                MeshFactory.CreateEllipsoid(size * 0.55f, size * 0.36f, size * 0.48f, 10, 7), material, parent);
            rock.transform.localPosition = position;
            rock.transform.localRotation = Quaternion.Euler(8f, 25f, -12f);
        }

        private static void CreateStreets(Transform root)
        {
            var roadRoot = new GameObject("Curving harbour streets and walkways");
            roadRoot.transform.SetParent(root, false);

            var mainPath = new List<Vector3>
            {
                new Vector3(-1.5f, 0.24f, -17f), new Vector3(-1.2f, 0.24f, -11f),
                new Vector3(0.0f, 0.24f, -4f), new Vector3(1.1f, 0.24f, 3f),
                new Vector3(2.5f, 0.24f, 10f), new Vector3(4.0f, 0.24f, 17f)
            };
            MeshFactory.MeshObject("Main coastal road", MeshFactory.CreateRibbon(mainPath, 4.8f, 0f),
                road, roadRoot.transform);

            var marketLane = new List<Vector3>
            {
                new Vector3(-15f, 0.25f, 7.2f), new Vector3(-10f, 0.25f, 5.2f),
                new Vector3(-5f, 0.25f, 3.8f), new Vector3(0f, 0.25f, 3.0f),
                new Vector3(8f, 0.25f, 2.8f), new Vector3(15f, 0.25f, 3.4f)
            };
            MeshFactory.MeshObject("Fish market lane", MeshFactory.CreateRibbon(marketLane, 4.0f, 0f),
                asphalt, roadRoot.transform);

            var ferryRoad = new List<Vector3>
            {
                new Vector3(1f, 0.25f, 4f), new Vector3(5f, 0.25f, 6f),
                new Vector3(10f, 0.25f, 7f), new Vector3(16f, 0.25f, 7.4f)
            };
            MeshFactory.MeshObject("Ferry approach", MeshFactory.CreateRibbon(ferryRoad, 3.7f, 0.012f),
                road, roadRoot.transform);

            MeshFactory.MeshObject("Main pavement left", MeshFactory.CreateRibbon(mainPath, 1.2f, 0.07f),
                paving, roadRoot.transform).transform.localPosition += Vector3.left * 3.0f;
            MeshFactory.MeshObject("Main pavement right", MeshFactory.CreateRibbon(mainPath, 1.2f, 0.07f),
                paving, roadRoot.transform).transform.localPosition += Vector3.right * 3.0f;

            // Broken lane markings rather than a single bright stripe.
            for (int i = 0; i < 7; i++)
            {
                float z = -14f + i * 4.7f;
                CreateStrip(roadRoot.transform, "Faded lane marker",
                    new Vector3(0.3f, 0.27f, z), new Vector3(0.3f, 0.27f, z + 1.8f),
                    0.08f, whitePaint);
            }

            // Curbs define the edges of the public square.
            CreateStrip(roadRoot.transform, "Market square curb A",
                new Vector3(-14f, 0.32f, 10.7f), new Vector3(14f, 0.32f, 10.7f), 0.18f, stone);
            CreateStrip(roadRoot.transform, "Market square curb B",
                new Vector3(-14f, 0.32f, -1.2f), new Vector3(14f, 0.32f, -1.2f), 0.18f, stone);

            // A paved open square in front of the fish market.
            var square = MeshFactory.MeshObject("Fish market square",
                CreateHorizontalQuad(18f, 10f, 0.30f), paving, roadRoot.transform);
            square.transform.localPosition = new Vector3(-8f, 0, 4.9f);
        }

        private static void CreateStrip(Transform parent, string name, Vector3 start, Vector3 end,
            float radius, Material material)
        {
            Vector3 delta = end - start;
            var strip = MeshFactory.MeshObject(name,
                MeshFactory.CreateTaperedCylinder(radius, radius, delta.magnitude, 8),
                material, parent);
            strip.transform.position = start;
            strip.transform.rotation = Quaternion.FromToRotation(Vector3.up, delta.normalized);
            strip.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        private static void CreateMarketBuildings(Transform root)
        {
            var buildings = new GameObject("Nambara architecture");
            buildings.transform.SetParent(root, false);

            CreateBuilding(buildings.transform, "Port authority office",
                new Vector3(-13f, 0.25f, -6f), 7.2f, 5.4f, 4.5f, 0.95f,
                plaster, terracotta, 2, true, false);
            CreateBuilding(buildings.transform, "Mara's fish market",
                new Vector3(-10.5f, 0.25f, 9.2f), 8.3f, 5.7f, 4.3f, 0.9f,
                paleBlue, slate, 2, true, true);
            CreateBuilding(buildings.transform, "Old town merchant house",
                new Vector3(10.4f, 0.25f, -8.3f), 7.2f, 6.1f, 5.0f, 1.1f,
                coral, terracotta, 2, true, false);
            CreateBuilding(buildings.transform, "Fisherman's cottage",
                new Vector3(11.0f, 0.25f, 10.0f), 6.5f, 5.3f, 3.9f, 0.85f,
                plaster, terracotta, 1, false, false);
            CreateBuilding(buildings.transform, "Harbour warehouse",
                new Vector3(18.2f, 0.25f, -0.3f), 8.2f, 7.1f, 4.7f, 0.45f,
                paleBlue, slate, 1, false, false);
            CreateBuilding(buildings.transform, "Hilltop residence",
                new Vector3(-3.2f, 0.25f, -15.4f), 6.8f, 5.6f, 4.5f, 1.0f,
                coral, slate, 2, true, false);
        }

        private static void CreateBuilding(Transform parent, string name, Vector3 position,
            float width, float depth, float height, float roofRise, Material wallMaterial,
            Material roofMaterial, int floors, bool balcony, bool shop)
        {
            var building = new GameObject(name);
            building.transform.SetParent(parent, false);
            building.transform.localPosition = position;

            MeshFactory.MeshObject("Hand-plastered gabled walls",
                MeshFactory.CreateGabledWalls(width, height, depth, roofRise),
                wallMaterial, building.transform, true);
            MeshFactory.MeshObject("Pitched roof with deep eaves",
                MeshFactory.CreateGabledRoof(width, height, depth, roofRise, 0.48f),
                roofMaterial, building.transform);

            // Foundation stones and continuous painted cornices add depth to the facade.
            CreateBeam(building.transform, "Foundation course",
                new Vector3(-width * 0.5f, 0.28f, depth * 0.5f + 0.07f),
                new Vector3(width * 0.5f, 0.28f, depth * 0.5f + 0.07f), 0.085f, stone);
            CreateBeam(building.transform, "Upper cornice",
                new Vector3(-width * 0.52f, height - 0.05f, depth * 0.5f + 0.08f),
                new Vector3(width * 0.52f, height - 0.05f, depth * 0.5f + 0.08f), 0.065f, whitePaint);

            int floorsToDraw = Mathf.Max(1, floors);
            for (int floor = 0; floor < floorsToDraw; floor++)
            {
                float y = floorsToDraw == 1 ? 1.75f : 1.15f + floor * 2.05f;
                float windowWidth = shop ? 0.88f : 0.72f;
                float windowHeight = shop ? 1.25f : 0.98f;
                float spacing = width * 0.25f;
                CreateWindow(building.transform, new Vector3(-spacing, y, depth * 0.5f + 0.065f),
                    windowWidth, windowHeight, floor == 1);
                CreateWindow(building.transform, new Vector3(spacing, y, depth * 0.5f + 0.065f),
                    windowWidth, windowHeight, floor == 1);
                if (floor == 0)
                    CreateDoor(building.transform, new Vector3(0, 0.2f, depth * 0.5f + 0.07f), 0.98f, 1.95f);
            }

            if (balcony && floorsToDraw > 1)
                CreateBalcony(building.transform, new Vector3(0, 2.05f, depth * 0.5f + 0.24f), width * 0.52f);

            if (shop)
            {
                CreateAwning(building.transform, new Vector3(0, 2.5f, depth * 0.5f + 0.56f), width * 0.9f);
                CreateSign(building.transform, "FISH MARKET", new Vector3(0, 3.2f, depth * 0.5f + 0.16f),
                    new Color(0.17f, 0.12f, 0.09f), paper);
            }
            else
            {
                CreateSign(building.transform, name.ToUpperInvariant(),
                    new Vector3(0, 2.65f, depth * 0.5f + 0.13f),
                    new Color(0.12f, 0.11f, 0.095f), gold);
            }

            // Roof ridge, chimney and rainwater downpipes.
            CreateBeam(building.transform, "Ridge cap",
                new Vector3(0, height + roofRise + 0.06f, -depth * 0.5f - 0.35f),
                new Vector3(0, height + roofRise + 0.06f, depth * 0.5f + 0.35f), 0.10f, roofMaterial);
            CreateTapered(building.transform, "Rainwater downpipe",
                new Vector3(width * 0.5f - 0.18f, 0.25f, depth * 0.5f + 0.13f),
                0.035f, 0.035f, height, darkMetal);
            if (width > 7.5f)
                CreateChimney(building.transform, new Vector3(-width * 0.32f, height + roofRise * 0.4f, -depth * 0.22f));
        }

        private static void CreateWindow(Transform parent, Vector3 center, float width, float height, bool shutters)
        {
            float halfW = width * 0.5f;
            float halfH = height * 0.5f;
            var v = new List<Vector3>();
            var t = new List<int>();
            MeshFactory.AddQuad(v, t,
                center + new Vector3(-halfW, -halfH, 0),
                center + new Vector3(halfW, -halfH, 0),
                center + new Vector3(halfW, halfH, 0),
                center + new Vector3(-halfW, halfH, 0));
            MeshFactory.MeshObject("Inset glass pane", MeshFactory.BuildMesh("Window glass", v, t),
                glass, parent);

            float frame = 0.045f;
            CreateBeam(parent, "Window frame", center + new Vector3(-halfW, -halfH, 0.03f),
                center + new Vector3(-halfW, halfH, 0.03f), frame, whitePaint);
            CreateBeam(parent, "Window frame", center + new Vector3(halfW, -halfH, 0.03f),
                center + new Vector3(halfW, halfH, 0.03f), frame, whitePaint);
            CreateBeam(parent, "Window sill", center + new Vector3(-halfW - 0.06f, -halfH, 0.04f),
                center + new Vector3(halfW + 0.06f, -halfH, 0.04f), frame, stone);
            CreateBeam(parent, "Window lintel", center + new Vector3(-halfW, halfH, 0.03f),
                center + new Vector3(halfW, halfH, 0.03f), frame, whitePaint);
            CreateBeam(parent, "Window crossbar", center + new Vector3(-halfW, 0, 0.045f),
                center + new Vector3(halfW, 0, 0.045f), 0.022f, whitePaint);
            if (shutters)
            {
                CreateBeam(parent, "Wooden shutter left", center + new Vector3(-halfW - 0.10f, -halfH, 0.025f),
                    center + new Vector3(-halfW - 0.10f, halfH, 0.025f), 0.055f, timber);
                CreateBeam(parent, "Wooden shutter right", center + new Vector3(halfW + 0.10f, -halfH, 0.025f),
                    center + new Vector3(halfW + 0.10f, halfH, 0.025f), 0.055f, timber);
            }
        }

        private static void CreateDoor(Transform parent, Vector3 center, float width, float height)
        {
            var v = new List<Vector3>();
            var t = new List<int>();
            MeshFactory.AddQuad(v, t,
                center + new Vector3(-width * 0.5f, 0, 0),
                center + new Vector3(width * 0.5f, 0, 0),
                center + new Vector3(width * 0.5f, height, 0),
                center + new Vector3(-width * 0.5f, height, 0));
            var door = MeshFactory.MeshObject("Heavy timber entrance", MeshFactory.BuildMesh("Door", v, t),
                timber, parent);
            CreateBeam(parent, "Door jamb", center, center + Vector3.up * height, 0.075f, stone);
            CreateBeam(parent, "Door jamb", center + Vector3.right * width,
                center + Vector3.right * width + Vector3.up * height, 0.075f, stone);
            CreateBeam(parent, "Door lintel", center + Vector3.up * height,
                center + Vector3.right * width + Vector3.up * height, 0.075f, stone);
            CreateBeam(parent, "Door handle", center + new Vector3(width * 0.28f, height * 0.48f, 0.04f),
                center + new Vector3(width * 0.28f, height * 0.48f, 0.13f), 0.025f, gold);
        }

        private static void CreateBalcony(Transform parent, Vector3 center, float width)
        {
            float half = width * 0.5f;
            CreateBeam(parent, "Balcony stone ledge", center + new Vector3(-half, 0, 0),
                center + new Vector3(half, 0, 0), 0.12f, stone);
            CreateBeam(parent, "Balcony handrail", center + new Vector3(-half, 0.85f, 0.45f),
                center + new Vector3(half, 0.85f, 0.45f), 0.045f, darkMetal);
            CreateBeam(parent, "Balcony lower rail", center + new Vector3(-half, 0.22f, 0.45f),
                center + new Vector3(half, 0.22f, 0.45f), 0.035f, darkMetal);
            for (int i = 0; i <= 6; i++)
            {
                float x = Mathf.Lerp(-half, half, i / 6f);
                CreateBeam(parent, "Balcony baluster", center + new Vector3(x, 0.18f, 0.45f),
                    center + new Vector3(x, 0.85f, 0.45f), 0.026f, darkMetal);
            }
            CreateBeam(parent, "Balcony return", center + new Vector3(-half, 0, 0),
                center + new Vector3(-half, 0.85f, 0.45f), 0.035f, darkMetal);
            CreateBeam(parent, "Balcony return", center + new Vector3(half, 0, 0),
                center + new Vector3(half, 0.85f, 0.45f), 0.035f, darkMetal);
        }

        private static void CreateAwning(Transform parent, Vector3 position, float width)
        {
            var v = new List<Vector3>();
            var t = new List<int>();
            Vector3 a = position + new Vector3(-width * 0.5f, 0, 0);
            Vector3 b = position + new Vector3(width * 0.5f, 0, 0);
            Vector3 c = position + new Vector3(width * 0.5f, -0.25f, 0.85f);
            Vector3 d = position + new Vector3(-width * 0.5f, -0.25f, 0.85f);
            MeshFactory.AddQuad(v, t, a, b, c, d);
            MeshFactory.MeshObject("Canvas market awning", MeshFactory.BuildMesh("Awning", v, t),
                coral, parent);
            for (int i = 0; i < 8; i++)
            {
                float x = Mathf.Lerp(-width * 0.5f, width * 0.5f, i / 7f);
                CreateBeam(parent, "Awning support", position + new Vector3(x, 0, 0),
                    position + new Vector3(x, -0.25f, 0.85f), 0.028f, timber);
            }
        }

        private static void CreateSign(Transform parent, string label, Vector3 position, Color background, Material trim)
        {
            var sign = MeshFactory.MeshObject("Painted shop sign",
                MeshFactory.CreateEllipsoid(0.5f, 0.5f, 0.5f, 12, 8), trim, parent);
            sign.transform.localPosition = position;
            sign.transform.localScale = new Vector3(2.2f, 0.34f, 0.075f);

            var textObject = new GameObject("Sign lettering / " + label);
            textObject.transform.SetParent(parent, false);
            textObject.transform.localPosition = position + new Vector3(0, -0.07f, 0.06f);
            textObject.transform.localRotation = Quaternion.identity;
            var text = textObject.AddComponent<TextMesh>();
            text.text = label;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.characterSize = 0.09f;
            text.fontSize = 36;
            text.color = background;
        }

        private static void CreateChimney(Transform parent, Vector3 position)
        {
            CreateTapered(parent, "Brick chimney", position, 0.24f, 0.19f, 1.3f, stone);
            CreateBeam(parent, "Chimney cap", position + new Vector3(-0.28f, 1.25f, 0),
                position + new Vector3(0.28f, 1.25f, 0), 0.055f, timber);
        }

        private static void CreateHarbour(Transform root)
        {
            var harbour = new GameObject("Working ferry pier");
            harbour.transform.SetParent(root, false);

            // Pier deck is a long timber mesh with closely spaced planks and visible piles.
            var deckV = new List<Vector3>();
            var deckT = new List<int>();
            MeshFactory.AddQuad(deckV, deckT,
                new Vector3(12f, 0.48f, 5.1f), new Vector3(24f, 0.48f, 5.1f),
                new Vector3(24f, 0.48f, 10.4f), new Vector3(12f, 0.48f, 10.4f));
            MeshFactory.MeshObject("Pier timber deck", MeshFactory.BuildMesh("Pier deck", deckV, deckT),
                timber, harbour.transform, true);

            for (int i = 0; i < 20; i++)
            {
                float x = 12.25f + i * 0.58f;
                CreateBeam(harbour.transform, "Deck plank seam",
                    new Vector3(x, 0.50f, 5.1f), new Vector3(x, 0.50f, 10.4f), 0.018f, sand);
            }

            for (int x = 12; x <= 24; x += 3)
            {
                CreateTapered(harbour.transform, "Pier pile", new Vector3(x, -0.52f, 5.35f),
                    0.15f, 0.11f, 1.9f, timber);
                CreateTapered(harbour.transform, "Pier pile", new Vector3(x, -0.52f, 10.15f),
                    0.15f, 0.11f, 1.9f, timber);
            }

            CreateBeam(harbour.transform, "Pier side rail",
                new Vector3(12.2f, 1.28f, 5.1f), new Vector3(23.8f, 1.28f, 5.1f), 0.07f, darkMetal);
            CreateBeam(harbour.transform, "Pier side rail",
                new Vector3(12.2f, 1.28f, 10.4f), new Vector3(23.8f, 1.28f, 10.4f), 0.07f, darkMetal);
            for (int x = 13; x < 24; x += 2)
            {
                CreateBeam(harbour.transform, "Pier rail post",
                    new Vector3(x, 0.48f, 5.1f), new Vector3(x, 1.28f, 5.1f), 0.04f, darkMetal);
                CreateBeam(harbour.transform, "Pier rail post",
                    new Vector3(x, 0.48f, 10.4f), new Vector3(x, 1.28f, 10.4f), 0.04f, darkMetal);
            }

            CreateBoat(harbour.transform, new Vector3(19.4f, 0.04f, 13.5f));
            CreateBollard(harbour.transform, new Vector3(13.2f, 0.52f, 6.0f));
            CreateBollard(harbour.transform, new Vector3(22.7f, 0.52f, 9.5f));
        }

        private static void CreateBoat(Transform parent, Vector3 position)
        {
            var boat = new GameObject("Ferry Six / moored fishing boat");
            boat.transform.SetParent(parent, false);
            boat.transform.localPosition = position;
            boat.transform.localRotation = Quaternion.Euler(0, -10f, 0);

            MeshFactory.MeshObject("Curved timber hull",
                MeshFactory.CreateBoatHull(7.4f, 2.25f, 1.0f), coral, boat.transform, true);
            CreateHorizontalAt(boat.transform, "Deck", 5.6f, 1.7f, 0.88f, timber);
            CreateTapered(boat.transform, "Mast", new Vector3(0, 0.85f, 0.5f),
                0.075f, 0.055f, 3.5f, darkMetal);

            var sailV = new List<Vector3>();
            var sailT = new List<int>();
            MeshFactory.AddTriangle(sailV, sailT,
                new Vector3(0.08f, 1.25f, 0.45f),
                new Vector3(0.08f, 4.0f, 0.45f),
                new Vector3(1.65f, 1.4f, 0.45f));
            MeshFactory.MeshObject("Faded triangular sail", MeshFactory.BuildMesh("Sail", sailV, sailT),
                paper, boat.transform);
            CreateTapered(boat.transform, "Bow rail", new Vector3(0, 0.9f, 3.3f),
                0.04f, 0.035f, 0.55f, timber);
        }

        private static void CreateHorizontalAt(Transform parent, string name, float width, float depth, float y, Material material)
        {
            var go = MeshFactory.MeshObject(name, CreateHorizontalQuad(width, depth, 0), material, parent);
            go.transform.localPosition = new Vector3(0, y, 0);
        }

        private static void CreateBollard(Transform parent, Vector3 position)
        {
            CreateTapered(parent, "Mooring bollard", position, 0.14f, 0.14f, 0.62f, darkMetal);
            CreateBeam(parent, "Bollard crossbar", position + new Vector3(-0.22f, 0.52f, 0),
                position + new Vector3(0.22f, 0.52f, 0), 0.07f, darkMetal);
        }

        private static void CreateVegetation(Transform root)
        {
            var plants = new GameObject("Coastal palms and vegetation");
            plants.transform.SetParent(root, false);
            CreatePalm(plants.transform, new Vector3(-19f, 0.18f, 6f), 4.9f);
            CreatePalm(plants.transform, new Vector3(-17f, 0.18f, -8f), 5.4f);
            CreatePalm(plants.transform, new Vector3(17.5f, 0.18f, -5f), 4.5f);
            CreatePalm(plants.transform, new Vector3(8f, 0.18f, 17f), 4.8f);
            CreateBroadleaf(plants.transform, new Vector3(-5f, 0.18f, 14f), 2.4f);
            CreateBroadleaf(plants.transform, new Vector3(15f, 0.18f, 15f), 2.0f);
            CreateBroadleaf(plants.transform, new Vector3(-21f, 0.18f, 14f), 2.3f);
        }

        private static void CreatePalm(Transform parent, Vector3 position, float height)
        {
            var palm = new GameObject("Wind-bent palm");
            palm.transform.SetParent(parent, false);
            palm.transform.localPosition = position;

            for (int i = 0; i < 5; i++)
            {
                Vector3 start = new Vector3(i * 0.04f, i * height / 5f, 0);
                Vector3 end = new Vector3(Mathf.Sin(i * 0.28f) * 0.42f, (i + 1) * height / 5f, 0);
                CreateBeam(palm.transform, "Curved palm trunk segment", start, end,
                    Mathf.Lerp(0.22f, 0.11f, (float)i / 5f), timber);
            }

            Vector3 crown = new Vector3(0.45f, height, 0);
            for (int i = 0; i < 9; i++)
            {
                float angle = i * Mathf.PI * 2f / 9f;
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0.18f + (i % 3) * 0.04f, Mathf.Sin(angle));
                Vector3 tip = crown + direction * 2.2f + Vector3.down * 0.72f;
                CreateFrond(palm.transform, crown, tip, 0.18f);
            }
        }

        private static void CreateFrond(Transform parent, Vector3 start, Vector3 end, float width)
        {
            Vector3 middle = Vector3.Lerp(start, end, 0.52f) + Vector3.down * 0.34f;
            Vector3 side = Vector3.Cross((end - start).normalized, Vector3.up).normalized * width;
            var v = new List<Vector3>();
            var t = new List<int>();
            MeshFactory.AddTriangle(v, t, start, middle + side, middle - side);
            MeshFactory.AddTriangle(v, t, middle - side, end, middle + side);
            MeshFactory.MeshObject("Palm leaf blade", MeshFactory.BuildMesh("Palm frond", v, t),
                palmLeaf, parent);
        }

        private static void CreateBroadleaf(Transform parent, Vector3 position, float scale)
        {
            var tree = new GameObject("Coastal shade tree");
            tree.transform.SetParent(parent, false);
            tree.transform.localPosition = position;
            CreateTapered(tree.transform, "Trunk", new Vector3(0, 0, 0), 0.22f, 0.11f, 2.5f, timber);

            var crown = MeshFactory.MeshObject("Rounded leaf canopy",
                MeshFactory.CreateEllipsoid(1f, 0.82f, 0.95f, 14, 10), foliage, tree.transform);
            crown.transform.localPosition = new Vector3(0, 2.75f, 0);
            crown.transform.localScale = Vector3.one * scale;
            var side = MeshFactory.MeshObject("Canopy side cluster",
                MeshFactory.CreateEllipsoid(0.75f, 0.62f, 0.8f, 12, 8), foliage, tree.transform);
            side.transform.localPosition = new Vector3(0.55f, 2.45f, 0.18f);
            side.transform.localScale = Vector3.one * (scale * 0.84f);
        }

        private static void CreateStreetFurniture(Transform root)
        {
            var props = new GameObject("Harbour street details");
            props.transform.SetParent(root, false);
            Vector3[] lampPositions =
            {
                new Vector3(-3.7f, 0.24f, -8f),
                new Vector3(4.8f, 0.24f, -1f),
                new Vector3(-3.6f, 0.24f, 8.8f),
                new Vector3(8f, 0.24f, 13f),
                new Vector3(-15.8f, 0.24f, 3.2f)
            };
            for (int i = 0; i < lampPositions.Length; i++) CreateLamp(props.transform, lampPositions[i]);

            CreateBench(props.transform, new Vector3(-5f, 0.27f, 0.7f));
            CreateCrateStack(props.transform, new Vector3(15.3f, 0.25f, 0.2f));
        }

        private static void CreateLamp(Transform parent, Vector3 position)
        {
            var lamp = new GameObject("Old harbour lantern");
            lamp.transform.SetParent(parent, false);
            lamp.transform.localPosition = position;
            CreateTapered(lamp.transform, "Tapered iron post", Vector3.zero, 0.075f, 0.045f, 3.4f, darkMetal);
            CreateBeam(lamp.transform, "Lamp arm", new Vector3(0, 3.15f, 0),
                new Vector3(0.42f, 3.35f, 0), 0.045f, darkMetal);
            var globe = MeshFactory.MeshObject("Warm lantern glass",
                MeshFactory.CreateEllipsoid(0.16f, 0.23f, 0.16f, 10, 8), signalGlow, lamp.transform);
            globe.transform.localPosition = new Vector3(0.44f, 3.13f, 0);
            var lightObject = new GameObject("Lantern point light");
            lightObject.transform.SetParent(lamp.transform, false);
            lightObject.transform.localPosition = new Vector3(0.44f, 3.13f, 0);
            var point = lightObject.AddComponent<Light>();
            point.type = LightType.Point;
            point.color = new Color(1f, 0.63f, 0.32f);
            point.intensity = 0.65f;
            point.range = 6f;
            point.shadows = LightShadows.None;
        }

        private static void CreateBench(Transform parent, Vector3 position)
        {
            var bench = new GameObject("Market square wooden bench");
            bench.transform.SetParent(parent, false);
            bench.transform.localPosition = position;
            for (int i = 0; i < 4; i++)
            {
                float z = i * 0.12f;
                CreateBeam(bench.transform, "Bench slat",
                    new Vector3(-0.8f, 0.58f, z), new Vector3(0.8f, 0.58f, z), 0.055f, timber);
            }
            CreateBeam(bench.transform, "Bench leg",
                new Vector3(-0.62f, 0.1f, 0.06f), new Vector3(-0.62f, 0.58f, 0.06f), 0.055f, darkMetal);
            CreateBeam(bench.transform, "Bench leg",
                new Vector3(0.62f, 0.1f, 0.06f), new Vector3(0.62f, 0.58f, 0.06f), 0.055f, darkMetal);
            CreateBeam(bench.transform, "Bench back support",
                new Vector3(-0.62f, 0.58f, 0.48f), new Vector3(-0.62f, 1.22f, 0.48f), 0.055f, darkMetal);
            CreateBeam(bench.transform, "Bench back support",
                new Vector3(0.62f, 0.58f, 0.48f), new Vector3(0.62f, 1.22f, 0.48f), 0.055f, darkMetal);
            CreateBeam(bench.transform, "Bench back slat",
                new Vector3(-0.8f, 1.0f, 0.48f), new Vector3(0.8f, 1.0f, 0.48f), 0.06f, timber);
        }

        private static void CreateCrateStack(Transform parent, Vector3 position)
        {
            var crates = new GameObject("Harbour cargo and fishing nets");
            crates.transform.SetParent(parent, false);
            crates.transform.localPosition = position;
            CreateCrate(crates.transform, new Vector3(0, 0.35f, 0));
            CreateCrate(crates.transform, new Vector3(0.72f, 0.35f, 0.18f));
            CreateCrate(crates.transform, new Vector3(0.32f, 1.05f, 0.05f));
            var net = MeshFactory.MeshObject("Coiled fishing net",
                MeshFactory.CreateTorus(0.42f, 0.055f, 20, 6), timber, crates.transform);
            net.transform.localPosition = new Vector3(-0.45f, 0.72f, 0.2f);
        }

        private static void CreateCrate(Transform parent, Vector3 position)
        {
            var crate = MeshFactory.MeshObject("Weathered cargo crate",
                MeshFactory.CreateGabledWalls(0.62f, 0.62f, 0.62f, 0f), timber, parent);
            crate.transform.localPosition = position;
            for (int i = 0; i < 4; i++)
            {
                float x = -0.25f + i * 0.16f;
                CreateBeam(parent, "Crate slat", position + new Vector3(x, 0.06f, 0.34f),
                    position + new Vector3(x, 0.57f, 0.34f), 0.024f, sand);
            }
        }

        private static void CreateLighthouse(Transform root)
        {
            var tower = new GameObject("North point signal lighthouse");
            tower.transform.SetParent(root, false);
            tower.transform.localPosition = new Vector3(20f, 0.15f, -15.8f);

            var body = MeshFactory.MeshObject("Tapered lighthouse stone tower",
                MeshFactory.CreateTaperedCylinder(1.05f, 0.76f, 8.2f, 24), plaster, tower.transform, true);
            body.transform.localPosition = Vector3.zero;

            CreateTapered(tower.transform, "Lantern room base", new Vector3(0, 8.1f, 0),
                0.95f, 0.95f, 0.28f, stone);
            CreateTapered(tower.transform, "Lantern room", new Vector3(0, 8.36f, 0),
                0.82f, 0.82f, 1.15f, glass);
            CreateTapered(tower.transform, "Lighthouse roof", new Vector3(0, 9.48f, 0),
                1.08f, 0.05f, 0.85f, terracotta);

            var beacon = MeshFactory.MeshObject("Signal lens",
                MeshFactory.CreateEllipsoid(0.38f, 0.38f, 0.38f, 16, 12), signalGlow, tower.transform);
            beacon.transform.localPosition = new Vector3(0, 8.95f, 0);
            var lightGO = new GameObject("Signal beam point light");
            lightGO.transform.SetParent(tower.transform, false);
            lightGO.transform.localPosition = new Vector3(0, 8.95f, 0);
            var beaconLight = lightGO.AddComponent<Light>();
            beaconLight.type = LightType.Point;
            beaconLight.color = new Color(1f, 0.46f, 0.2f);
            beaconLight.intensity = 2.2f;
            beaconLight.range = 18f;
            beaconLight.shadows = LightShadows.None;

            for (int i = 0; i < 12; i++)
            {
                float angle = i * Mathf.PI * 2f / 12f;
                Vector3 start = new Vector3(Mathf.Cos(angle) * 0.88f, 8.28f, Mathf.Sin(angle) * 0.88f);
                Vector3 end = new Vector3(Mathf.Cos(angle) * 0.88f, 9.25f, Mathf.Sin(angle) * 0.88f);
                CreateBeam(tower.transform, "Lantern gallery railing", start, end, 0.028f, darkMetal);
            }
            CreateBeam(tower.transform, "Gallery handrail",
                new Vector3(-0.95f, 9.23f, 0), new Vector3(0.95f, 9.23f, 0), 0.035f, darkMetal);
        }

        private static void CreateInvestigationSites(Transform root, MissionManager mission)
        {
            CreateClue(root, mission, 0, "Port ledger",
                new Vector3(-12.0f, 0.35f, -2.0f),
                "The harbour ledger shows your brother booked passage on Ferry Six.");
            CreateClue(root, mission, 1, "Fish market radio",
                new Vector3(-5.8f, 0.35f, 7.8f),
                "A fisherman heard a voice repeat the word AFTERLIGHT over the radio.");
            CreateClue(root, mission, 2, "Old town photograph",
                new Vector3(8.0f, 0.35f, -5.2f),
                "The photograph places your brother beside a black van near the old pier.");
            CreateClue(root, mission, 3, "Ferry ticket",
                new Vector3(18.0f, 0.55f, 8.3f),
                "The ticket carries a hand-written route ending at the northern signal tower.");

            var towerPoint = new GameObject("Signal tower investigation point");
            towerPoint.transform.SetParent(root, false);
            towerPoint.transform.localPosition = new Vector3(20f, 0.4f, -12.6f);
            var sphere = towerPoint.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 2.3f;
            var finale = towerPoint.AddComponent<SignalTowerInteractable>();
            finale.Configure(mission);
        }

        private static void CreateClue(Transform root, MissionManager mission, int index,
            string label, Vector3 position, string message)
        {
            var clue = new GameObject("Evidence / " + label);
            clue.transform.SetParent(root, false);
            clue.transform.localPosition = position;

            if (index == 0)
            {
                var ledger = MeshFactory.MeshObject("Open harbour ledger",
                    MeshFactory.CreateEllipsoid(0.38f, 0.035f, 0.28f, 12, 8), paper, clue.transform);
                ledger.transform.localRotation = Quaternion.Euler(0, 12f, 0);
                CreateBeam(clue.transform, "Ledger spine", new Vector3(-0.02f, 0.04f, -0.27f),
                    new Vector3(-0.02f, 0.04f, 0.27f), 0.035f, timber);
            }
            else if (index == 1)
            {
                var radio = MeshFactory.MeshObject("Old portable radio",
                    MeshFactory.CreateEllipsoid(0.28f, 0.16f, 0.15f, 12, 8), darkMetal, clue.transform);
                radio.transform.localPosition = new Vector3(0, 0.18f, 0);
                CreateTapered(clue.transform, "Radio aerial", new Vector3(0.18f, 0.25f, -0.05f),
                    0.018f, 0.008f, 0.65f, darkMetal);
                var dial = MeshFactory.MeshObject("Radio tuning dial",
                    MeshFactory.CreateEllipsoid(0.055f, 0.055f, 0.025f, 10, 8), gold, clue.transform);
                dial.transform.localPosition = new Vector3(-0.12f, 0.2f, 0.13f);
            }
            else if (index == 2)
            {
                var photo = MeshFactory.MeshObject("Old photograph",
                    MeshFactory.CreateEllipsoid(0.29f, 0.018f, 0.22f, 12, 8), paper, clue.transform);
                photo.transform.localPosition = new Vector3(0, 0.03f, 0);
                photo.transform.localRotation = Quaternion.Euler(0, -8f, 0);
            }
            else
            {
                var ticket = MeshFactory.MeshObject("Ferry ticket",
                    MeshFactory.CreateEllipsoid(0.34f, 0.02f, 0.13f, 12, 8), paper, clue.transform);
                ticket.transform.localPosition = new Vector3(0, 0.02f, 0);
                ticket.transform.localRotation = Quaternion.Euler(0, 14f, 0);
            }

            var marker = MeshFactory.MeshObject("Subtle investigation marker",
                MeshFactory.CreateTorus(0.42f, 0.025f, 20, 6), gold, clue.transform);
            marker.transform.localPosition = new Vector3(0, 0.02f, 0);
            marker.transform.localRotation = Quaternion.Euler(90f, 0, 0);

            var interactable = clue.AddComponent<ClueInteractable>();
            interactable.Configure(mission, index, label, message);
            var collider = clue.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 2.2f;
        }

        private static void CreateMara(Transform root, GameUI ui)
        {
            var mara = CharacterFactory.CreateMara(root);
            mara.transform.localPosition = new Vector3(-5.6f, 0.3f, 9.5f);
            mara.transform.localRotation = Quaternion.Euler(0, -25f, 0);
            var npc = mara.AddComponent<DialogueNPC>();
            var player = Object.FindFirstObjectByType<PlayerMotor>();
            npc.Configure(ui, player);
            var trigger = mara.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 1.2f;
        }

        private static void CreateBeam(Transform parent, string name, Vector3 start, Vector3 end,
            float radius, Material material)
        {
            Vector3 delta = end - start;
            var beam = MeshFactory.MeshObject(name,
                MeshFactory.CreateTaperedCylinder(radius, radius * 0.9f, 1f, 8), material, parent);
            beam.transform.localPosition = start;
            beam.transform.localRotation = Quaternion.FromToRotation(Vector3.up, delta.normalized);
            beam.transform.localScale = new Vector3(1f, delta.magnitude, 1f);
        }

        private static void CreateTapered(Transform parent, string name, Vector3 basePosition,
            float bottomRadius, float topRadius, float height, Material material)
        {
            var part = MeshFactory.MeshObject(name,
                MeshFactory.CreateTaperedCylinder(bottomRadius, topRadius, height, 12), material, parent);
            part.transform.localPosition = basePosition;
        }
    }
}
