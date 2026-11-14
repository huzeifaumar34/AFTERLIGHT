using UnityEngine;

namespace Afterlight
{
    public static class CharacterFactory
    {
        public static GameObject CreateElias(Transform parent)
        {
            var jacket = MaterialFactory.Create("Elias / sea-worn jacket", new Color(0.16f, 0.24f, 0.27f), 0f, 0.18f);
            var shirt = MaterialFactory.Create("Elias / shirt", new Color(0.76f, 0.68f, 0.54f), 0f, 0.12f);
            var trousers = MaterialFactory.Create("Elias / trousers", new Color(0.20f, 0.22f, 0.23f), 0f, 0.1f);
            var skin = MaterialFactory.Create("Warm skin", new Color(0.58f, 0.36f, 0.24f), 0f, 0.1f);
            var hair = MaterialFactory.Create("Dark hair", new Color(0.065f, 0.052f, 0.045f), 0f, 0.06f);
            var boots = MaterialFactory.Create("Worn leather", new Color(0.12f, 0.09f, 0.075f), 0.05f, 0.22f);
            var accent = MaterialFactory.Create("Canvas strap", new Color(0.60f, 0.40f, 0.22f), 0f, 0.12f);
            return CreateHumanoid("Elias", parent, jacket, shirt, trousers, skin, hair, boots, accent, false);
        }

        public static GameObject CreateMara(Transform parent)
        {
            var coat = MaterialFactory.Create("Mara / teal coat", new Color(0.12f, 0.31f, 0.31f), 0f, 0.22f);
            var blouse = MaterialFactory.Create("Mara / blouse", new Color(0.78f, 0.68f, 0.52f), 0f, 0.12f);
            var skirt = MaterialFactory.Create("Mara / skirt", new Color(0.18f, 0.25f, 0.27f), 0f, 0.08f);
            var skin = MaterialFactory.Create("Mara / skin", new Color(0.68f, 0.44f, 0.31f), 0f, 0.12f);
            var hair = MaterialFactory.Create("Mara / dark curls", new Color(0.10f, 0.065f, 0.05f), 0f, 0.08f);
            var boots = MaterialFactory.Create("Mara / boots", new Color(0.15f, 0.11f, 0.09f), 0.02f, 0.2f);
            var scarf = MaterialFactory.Create("Mara / rust scarf", new Color(0.58f, 0.19f, 0.12f), 0f, 0.12f);
            return CreateHumanoid("Mara", parent, coat, blouse, skirt, skin, hair, boots, scarf, true);
        }

        private static GameObject CreateHumanoid(string name, Transform parent,
            Material outer, Material inner, Material legsMaterial, Material skin,
            Material hair, Material boots, Material accent, bool feminine)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);

            // Torso uses a tapered mesh and layered clothing, not a cuboid body.
            AddEllipsoid(root.transform, "Ribcage", Vector3.zero + Vector3.up * 1.12f,
                new Vector3(0.31f, 0.43f, 0.20f), outer);
            AddEllipsoid(root.transform, "Inner shirt", new Vector3(0, 1.18f, 0.205f),
                new Vector3(0.17f, 0.30f, 0.035f), inner);
            AddTapered(root.transform, "Neck", new Vector3(0, 1.52f, 0),
                0.095f, 0.09f, 0.18f, skin);

            if (feminine)
            {
                AddTapered(root.transform, "Long coat hem", new Vector3(0, 0.68f, 0),
                    0.28f, 0.19f, 0.48f, outer);
                AddEllipsoid(root.transform, "Shoulder scarf", new Vector3(0, 1.39f, 0.03f),
                    new Vector3(0.32f, 0.09f, 0.22f), accent);
            }
            else
            {
                AddEllipsoid(root.transform, "Jacket hem", new Vector3(0, 0.82f, -0.01f),
                    new Vector3(0.30f, 0.22f, 0.20f), outer);
                AddEllipsoid(root.transform, "Shoulder strap", new Vector3(0.17f, 1.14f, 0.07f),
                    new Vector3(0.045f, 0.34f, 0.035f), accent);
                AddEllipsoid(root.transform, "Satchel", new Vector3(-0.27f, 0.94f, -0.12f),
                    new Vector3(0.15f, 0.21f, 0.09f), accent);
            }

            // Head, jaw, ears, nose and eyes form a readable human face.
            AddEllipsoid(root.transform, "Head", new Vector3(0, 1.76f, 0.015f),
                new Vector3(0.205f, 0.255f, 0.19f), skin);
            AddEllipsoid(root.transform, "Hair crown", new Vector3(0, 1.94f, -0.005f),
                new Vector3(0.218f, 0.14f, 0.205f), hair);
            AddEllipsoid(root.transform, "Hair back", new Vector3(0, 1.77f, -0.13f),
                new Vector3(0.205f, 0.24f, 0.10f), hair);
            AddEllipsoid(root.transform, "Left ear", new Vector3(-0.205f, 1.76f, 0),
                new Vector3(0.055f, 0.075f, 0.045f), skin);
            AddEllipsoid(root.transform, "Right ear", new Vector3(0.205f, 1.76f, 0),
                new Vector3(0.055f, 0.075f, 0.045f), skin);
            AddEllipsoid(root.transform, "Nose", new Vector3(0, 1.75f, 0.195f),
                new Vector3(0.045f, 0.06f, 0.055f), skin);

            var eye = MaterialFactory.Create("Eyes", new Color(0.06f, 0.055f, 0.05f), 0f, 0.1f);
            AddEllipsoid(root.transform, "Left eye", new Vector3(-0.075f, 1.81f, 0.18f),
                new Vector3(0.025f, 0.03f, 0.018f), eye);
            AddEllipsoid(root.transform, "Right eye", new Vector3(0.075f, 1.81f, 0.18f),
                new Vector3(0.025f, 0.03f, 0.018f), eye);

            var leftArm = CreateLimb(root.transform, "Left arm", new Vector3(-0.34f, 1.38f, 0),
                outer, skin, false);
            var rightArm = CreateLimb(root.transform, "Right arm", new Vector3(0.34f, 1.38f, 0),
                outer, skin, false);
            var leftLeg = CreateLimb(root.transform, "Left leg", new Vector3(-0.15f, 0.61f, 0),
                legsMaterial, boots, true);
            var rightLeg = CreateLimb(root.transform, "Right leg", new Vector3(0.15f, 0.61f, 0),
                legsMaterial, boots, true);

            var animator = root.AddComponent<HumanoidRigAnimator>();
            animator.leftArm = leftArm;
            animator.rightArm = rightArm;
            animator.leftLeg = leftLeg;
            animator.rightLeg = rightLeg;
            return root;
        }

        private static Transform CreateLimb(Transform parent, string name, Vector3 position,
            Material cloth, Material endMaterial, bool leg)
        {
            var pivot = new GameObject(name + " pivot").transform;
            pivot.SetParent(parent, false);
            pivot.localPosition = position;

            float length = leg ? 0.66f : 0.58f;
            AddEllipsoid(pivot, name + " upper", new Vector3(0, -length * 0.30f, 0),
                leg ? new Vector3(0.14f, 0.29f, 0.14f) : new Vector3(0.12f, 0.27f, 0.12f), cloth);
            AddEllipsoid(pivot, name + " joint", new Vector3(0, -length * 0.60f, 0),
                new Vector3(0.105f, 0.12f, 0.11f), cloth);
            AddEllipsoid(pivot, name + " lower", new Vector3(0, -length * 0.84f, 0),
                new Vector3(0.095f, 0.22f, 0.10f), cloth);

            if (leg)
            {
                AddEllipsoid(pivot, name + " boot", new Vector3(0, -0.66f, 0.075f),
                    new Vector3(0.115f, 0.105f, 0.19f), endMaterial);
            }
            else
            {
                AddEllipsoid(pivot, name + " hand", new Vector3(0, -0.60f, 0.01f),
                    new Vector3(0.095f, 0.10f, 0.09f), endMaterial);
            }
            return pivot;
        }

        private static void AddEllipsoid(Transform parent, string name, Vector3 position,
            Vector3 scale, Material material)
        {
            var go = MeshFactory.MeshObject(name,
                MeshFactory.CreateEllipsoid(0.5f, 0.5f, 0.5f, 14, 10), material, parent);
            go.transform.localPosition = position;
            go.transform.localScale = scale * 2f;
        }

        private static void AddTapered(Transform parent, string name, Vector3 position,
            float bottom, float top, float height, Material material)
        {
            var go = MeshFactory.MeshObject(name,
                MeshFactory.CreateTaperedCylinder(bottom, top, height, 14), material, parent);
            go.transform.localPosition = position - Vector3.up * (height * 0.5f);
        }
    }
}
