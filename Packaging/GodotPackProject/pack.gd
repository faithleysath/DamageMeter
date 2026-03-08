extends SceneTree


func _init() -> void:
    var args := OS.get_cmdline_user_args()
    if args.size() < 2:
        printerr("Expected output .pck path and manifest path.")
        quit(1)
        return

    var output_path := args[0]
    var manifest_path := args[1]
    var image_path := args[2] if args.size() >= 3 else ""
    var en_loc_path := args[3] if args.size() >= 4 else ""
    var zhs_loc_path := args[4] if args.size() >= 5 else ""
    var image_import_path := args[5] if args.size() >= 6 else ""
    var image_ctex_path := args[6] if args.size() >= 7 else ""
    var source_path := ProjectSettings.globalize_path("res://placeholder.txt")
    var manifest_json := FileAccess.get_file_as_string(manifest_path)
    var manifest_data = JSON.parse_string(manifest_json)
    var pck_name := "DamageMeter"
    var packer := PCKPacker.new()

    if typeof(manifest_data) == TYPE_DICTIONARY and manifest_data.has("pck_name"):
        pck_name = str(manifest_data["pck_name"])

    var err := packer.pck_start(output_path)
    if err != OK:
        printerr("Failed to start PCK: %s" % err)
        quit(err)
        return

    err = packer.add_file("res://%s.placeholder" % pck_name, source_path)
    if err != OK:
        printerr("Failed to add placeholder file: %s" % err)
        quit(err)
        return

    err = packer.add_file("res://mod_manifest.json", manifest_path)
    if err != OK:
        printerr("Failed to add mod manifest: %s" % err)
        quit(err)
        return

    if image_path != "" and FileAccess.file_exists(image_path):
        err = packer.add_file("res://%s/mod_image.png" % pck_name, image_path)
        if err != OK:
            printerr("Failed to add mod image: %s" % err)
            quit(err)
            return

    if image_import_path != "" and FileAccess.file_exists(image_import_path):
        err = packer.add_file("res://%s/mod_image.png.import" % pck_name, image_import_path)
        if err != OK:
            printerr("Failed to add mod image import metadata: %s" % err)
            quit(err)
            return

    if image_ctex_path != "" and FileAccess.file_exists(image_ctex_path):
        err = packer.add_file("res://.godot/imported/%s" % image_ctex_path.get_file(), image_ctex_path)
        if err != OK:
            printerr("Failed to add mod image imported texture: %s" % err)
            quit(err)
            return

    if en_loc_path != "" and FileAccess.file_exists(en_loc_path):
        err = packer.add_file("res://%s/localization/en.json" % pck_name, en_loc_path)
        if err != OK:
            printerr("Failed to add English localization: %s" % err)
            quit(err)
            return

    if zhs_loc_path != "" and FileAccess.file_exists(zhs_loc_path):
        err = packer.add_file("res://%s/localization/zhs.json" % pck_name, zhs_loc_path)
        if err != OK:
            printerr("Failed to add Simplified Chinese localization: %s" % err)
            quit(err)
            return

    err = packer.flush(true)
    if err != OK:
        printerr("Failed to flush PCK: %s" % err)
        quit(err)
        return

    print("Wrote mod pack to %s" % output_path)
    quit(0)
