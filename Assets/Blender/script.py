import csv
from mathutils import Quaternion, Vector, Matrix
import bpy

path_to_csv = 'C:\\Users\\chart\\Desktop\\frames.csv'
scene = bpy.context.scene
scene_collection = bpy.context.scene.collection
scene.frame_start = 0
bpy.ops.screen.frame_jump(end=False)
arm = bpy.data.objects['Armature']

#keys = [0,11,12,13,14,15,16,21,22,23,24,25,26,27,28,31,32]
keys = [0,13,14,15,16,23,24,31,32]

# clear test_markers
for c in bpy.data.collections:
    if c.name == "test_markers":
        bpy.data.collections.remove(bpy.data.collections['test_markers'])

# create collection test_markers
markers = bpy.data.collections.new("test_markers")
bpy.context.scene.collection.children.link(markers)

with open(path_to_csv, newline='') as csvfile:
    csvreader = csv.reader(csvfile, delimiter=';', quotechar=';', skipinitialspace=True)
    
    # rows
    for row_index, row in enumerate(csvreader):

        # landmarks
        for l in row:
            coords = l.split(':')
            
            # coordinates
            for coords_index, c in enumerate(coords):
                if coords_index == 0:
                    coords[coords_index] = int(c)
                else:    
                    # [0]-idx, [1]-x, [2]-y, [3]-z
                    coords[coords_index] = float(c.replace(',','.'))
                    
            if coords[0] in keys:
                # Blender global: x,-z,-y
                #bone_location = Vector((coords[1] - 2.2, coords[3] + 0.1, -coords[2] + 3))
                bone_location = Vector((coords[1] - 2.2, 0, -coords[2] + 3))
                
                # Test
                if row_index == 0:
                    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.05, location=bone_location)
                    bpy.context.active_object.name = 'Marker_{}'.format(str(coords[0]))
                    markers.objects.link(bpy.context.active_object)
                    scene_collection.objects.unlink(bpy.context.active_object)
                
                bone = arm.pose.bones[str(coords[0])]
                v = bone_location - arm.location
                bone.location = bone.bone.matrix_local.inverted() @ v
                
                arm.keyframe_insert(data_path='pose.bones["{}"].location'.format(str(coords[0])), frame=row_index)
   
    print('script done')