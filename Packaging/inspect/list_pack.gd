extends SceneTree

func walk(path: String) -> void:
    var dir := DirAccess.open(path)
    if dir == null:
        return
    dir.list_dir_begin()
    while true:
        var name := dir.get_next()
        if name == "":
            break
        if name == "." or name == "..":
            continue
        var full := path.path_join(name)
        if dir.current_is_dir():
            walk(full)
        else:
            print(full)
    dir.list_dir_end()

func _init() -> void:
    var args := OS.get_cmdline_user_args()
    if args.is_empty():
        printerr("need pack path")
        quit(1)
        return
    if !ProjectSettings.load_resource_pack(args[0], true):
        printerr("failed to load pack")
        quit(2)
        return
    walk("res://")
    quit()
