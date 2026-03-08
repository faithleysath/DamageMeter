extends SceneTree

func _init() -> void:
    var args := OS.get_cmdline_user_args()
    if args.size() < 3:
        printerr("need pack path, source path, output path")
        quit(1)
        return
    var pack_path := args[0]
    var source_path := args[1]
    var output_path := args[2]
    if !ProjectSettings.load_resource_pack(pack_path, true):
        printerr("failed to load pack")
        quit(2)
        return
    var file := FileAccess.open(source_path, FileAccess.READ)
    if file == null:
        printerr("failed to open " + source_path)
        quit(3)
        return
    var data := file.get_buffer(file.get_length())
    var out := FileAccess.open(output_path, FileAccess.WRITE)
    if out == null:
        printerr("failed to open output")
        quit(4)
        return
    out.store_buffer(data)
    quit()
