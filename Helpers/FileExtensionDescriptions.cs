using System.ComponentModel;

namespace ColumnExplorer.Helpers
{
    public enum FileExtensionDescriptions
    {
        [Description("Text Document")]
        txt,
        [Description("Portable Document Format")]
        pdf,
        [Description("Microsoft Word Document")]
        docx,
        [Description("Microsoft Excel Spreadsheet")]
        xlsx,
        [Description("Image File")]
        jpg,
        [Description("Image File")]
        png,
        [Description("Rich Text Format")]
        rtf,
        [Description("HyperText Markup Language")]
        html,
        [Description("Cascading Style Sheets")]
        css,
        [Description("JavaScript File")]
        js,
        [Description("Java Source File")]
        java,
        [Description("Python Script")]
        py,
        [Description("C# Source File")]
        cs,
        [Description("C++ Source File")]
        cpp,
        [Description("Comma-Separated Values")]
        csv,
        [Description("Extensible Markup Language")]
        xml,
        [Description("JSON File")]
        json,
        [Description("ZIP Archive")]
        zip,
        [Description("GZIP Archive")]
        gz,
        [Description("Executable File")]
        exe,
        [Description("Batch File")]
        bat,
        [Description("Shell Script")]
        sh,
        [Description("Markdown Document")]
        md,
        [Description("Graphics Interchange Format")]
        gif,
        [Description("Bitmap Image File")]
        bmp,
        [Description("Audio File")]
        mp3,
        [Description("Video File")]
        mp4,
        [Description("Windows Media Audio")]
        wma,
        [Description("Windows Media Video")]
        wmv,
        [Description("Adobe Photoshop Document")]
        psd,
        [Description("Scalable Vector Graphics")]
        svg,
        [Description("TrueType Font")]
        ttf,
        [Description("OpenType Font")]
        otf,
        [Description("Microsoft PowerPoint Presentation")]
        pptx,
        [Description("Microsoft Access Database")]
        accdb,
        [Description("SQL Script")]
        sql,
        [Description("Log File")]
        log,
        [Description("Configuration File")]
        config,
        [Description("YAML File")]
        yaml,
        [Description("TAR Archive")]
        tar,
        [Description("7-Zip Archive")]
        _7z,
        [Description("Apple Disk Image")]
        dmg,
        [Description("ISO Disk Image")]
        iso,
        [Description("Windows Installer Package")]
        msi,
        [Description("Torrent File")]
        torrent,
        [Description("Adobe Illustrator File")]
        ai,
        [Description("MPEG Video File")]
        mpeg,
        [Description("FLAC Audio File")]
        flac,
        [Description("Waveform Audio File")]
        wav,
        [Description("Ogg Vorbis Audio File")]
        ogg,
        [Description("Matroska Video File")]
        mkv,
        [Description("QuickTime Movie")]
        mov,
        [Description("PHP Script")]
        php,
        [Description("Ruby Script")]
        rb,
        [Description("Perl Script")]
        pl,
        [Description("ASP.NET File")]
        aspx,
        [Description("Visual Basic Script")]
        vbs,
        [Description("Scala Source File")]
        scala,
        [Description("Kotlin Source File")]
        kt,
        [Description("Rust Source File")]
        rs,
        [Description("Go Source File")]
        go,
        [Description("Swift Source File")]
        swift,
        [Description("Objective-C Source File")]
        m,
        [Description("TypeScript File")]
        ts,
        [Description("CoffeeScript File")]
        coffee,
        [Description("Dart Source File")]
        dart,
        [Description("Haskell Source File")]
        hs,
        [Description("Lua Script")]
        lua,
        [Description("R Script")]
        r,
        [Description("Shell Script")]
        zsh,
        [Description("PowerShell Script")]
        ps1,
        [Description("AppleScript File")]
        scpt,
        [Description("F# Source File")]
        fs,
        [Description("Erlang Source File")]
        erl,
        [Description("Elixir Source File")]
        ex,
        [Description("Clojure Source File")]
        clj,
        [Description("Scheme Source File")]
        scm,
        [Description("Common Lisp Source File")]
        lisp,
        [Description("Fortran Source File")]
        f90,
        [Description("COBOL Source File")]
        cob,
        [Description("Ada Source File")]
        ada,
        [Description("Pascal Source File")]
        pas,
        [Description("VHDL Source File")]
        vhdl,
        [Description("Verilog Source File")]
        v,
        [Description("Assembly Source File")]
        asm,
        [Description("Makefile")]
        makefile,
        [Description("CMake File")]
        cmake,
        [Description("Gradle Build File")]
        gradle,
        [Description("Maven POM File")]
        pom,
        [Description("Ant Build File")]
        build,
        [Description("Dockerfile")]
        dockerfile,
        [Description("Kubernetes YAML File")]
        k8s,
        [Description("Terraform File")]
        tf,
        [Description("Ansible Playbook")]
        yml,
        [Description("Puppet Manifest")]
        pp,
        [Description("Salt State File")]
        sls,
        [Description("Vagrantfile")]
        vagrantfile,
        [Description("Jupyter Notebook")]
        ipynb,
        [Description("R Markdown File")]
        rmd,
        [Description("LaTeX Document")]
        tex,
        [Description("BibTeX Bibliography File")]
        bib,
        [Description("SPSS Syntax File")]
        sps,
        [Description("SAS Program File")]
        sas,
        [Description("R Data File")]
        rdata,
        [Description("MATLAB Data File")]
        mat,
        [Description("HDF5 File")]
        h5,
        [Description("NetCDF File")]
        nc,
        [Description("GRIB File")]
        grb,
        [Description("GeoTIFF File")]
        tif,
        [Description("Shapefile")]
        shp,
        [Description("KML File")]
        kml,
        [Description("GPX File")]
        gpx,
        [Description("OpenStreetMap File")]
        osm,
        [Description("QGIS Project File")]
        qgs,
        [Description("ArcGIS Project File")]
        aprx,
        [Description("MapInfo File")]
        tab,
        [Description("ERDAS Imagine File")]
        img,
        [Description("ENVI File")]
        hdr,
        [Description("IDL Script")]
        pro,
        [Description("IDL Save File")]
        sav,
        [Description("IDL Data File")]
        dat,
        [Description("IDL Function File")]
        fun,
        [Description("IDL Main File")]
        main,
        [Description("IDL Include File")]
        inc,
        [Description("IDL Library File")]
        lib,
        [Description("IDL Module File")]
        mod,
        [Description("IDL Package File")]
        pkg,
        [Description("IDL Project File")]
        prj,
        [Description("IDL Workspace File")]
        wsp,
        [Description("IDL Configuration File")]
        cfg,
        [Description("IDL Error File")]
        err,
        [Description("IDL Warning File")]
        warn,
        [Description("IDL Info File")]
        info,
        [Description("IDL Debug File")]
        dbg,
        [Description("IDL Trace File")]
        trc,
        [Description("IDL Profile File")]
        prof,
        [Description("IDL Test File")]
        tst,
        [Description("IDL Sample File")]
        samp,
        [Description("IDL Template File")]
        tpl,
        [Description("IDL Documentation File")]
        doc,
        [Description("IDL Readme File")]
        readme,
        [Description("IDL License File")]
        lic,
        [Description("IDL Manifest File")]
        mf,
        [Description("IDL Metadata File")]
        meta,
        [Description("IDL Resource File")]
        res,
        [Description("IDL Asset File")]
        asset,
        [Description("IDL Data File")]
        data,
        [Description("IDL Settings File")]
        settings,
        [Description("IDL Preferences File")]
        prefs,
        [Description("IDL Options File")]
        opts,
        [Description("IDL Properties File")]
        props,
        [Description("IDL Arguments File")]
        args,
        [Description("IDL Variables File")]
        vars,
        [Description("IDL Constants File")]
        consts,
        [Description("IDL Types File")]
        types,
        [Description("IDL Classes File")]
        classes,
        [Description("IDL Interfaces File")]
        ifaces,
        [Description("IDL Enums File")]
        enums,
        [Description("IDL Structs File")]
        structs,
        [Description("IDL Unions File")]
        unions,
        [Description("IDL Functions File")]
        funcs,
        [Description("IDL Procedures File")]
        procs,
        [Description("IDL Methods File")]
        methods,
        [Description("IDL Fields File")]
        fields,
        [Description("IDL Attributes File")]
        attrs,
        [Description("IDL Annotations File")]
        annos,
        [Description("IDL Directives File")]
        dirs,
        [Description("IDL Macros File")]
        macros,
        [Description("IDL Includes File")]
        includes,
        [Description("IDL Imports File")]
        imports,
        [Description("IDL Exports File")]
        exports,
        [Description("IDL Dependencies File")]
        deps,
        [Description("IDL References File")]
        refs,
        [Description("IDL Links File")]
        links,
        [Description("IDL Tags File")]
        tags,
        [Description("IDL Labels File")]
        labels,
        [Description("IDL Comments File")]
        comments,
        [Description("IDL Notes File")]
        notes,
        [Description("IDL Remarks File")]
        remarks,
        [Description("IDL Descriptions File")]
        descs,
        [Description("IDL Summaries File")]
        sums,
        [Description("IDL Details File")]
        details,
        [Description("IDL Examples File")]
        exs,
        [Description("IDL Samples File")]
        samples,
        [Description("IDL Templates File")]
        templates,
        [Description("IDL Snippets File")]
        snippets,
        [Description("IDL Fragments File")]
        frags,
        [Description("IDL Parts File")]
        parts,
        [Description("IDL Sections File")]
        sections,
        [Description("IDL Chapters File")]
        chapters,
        [Description("IDL Volumes File")]
        volumes,
        [Description("IDL Books File")]
        books,
        [Description("IDL Articles File")]
        articles,
        [Description("IDL Papers File")]
        papers,
        [Description("IDL Reports File")]
        reports,
        [Description("IDL Theses File")]
        theses,
        [Description("IDL Dissertations File")]
        diss,
        [Description("IDL Manuscripts File")]
        mss,
        [Description("IDL Drafts File")]
        drafts,
        [Description("IDL Revisions File")]
        revs,
        [Description("IDL Versions File")]
        vers,
        [Description("IDL Editions File")]
        eds,
        [Description("IDL Releases File")]
        rels,
        [Description("IDL Updates File")]
        updates,
        [Description("IDL Patches File")]
        patches,
        [Description("IDL Fixes File")]
        fixes,
        [Description("IDL Changes File")]
        changes,
        [Description("IDL Modifications File")]
        mods,
        [Description("IDL Enhancements File")]
        enhs,
        [Description("IDL Improvements File")]
        imps,
        [Description("IDL Features File")]
        feats,
        [Description("IDL Capabilities File")]
        caps,
        [Description("IDL Operations File")]
        ops,
        [Description("IDL Actions File")]
        acts,
        [Description("IDL Tasks File")]
        tasks,
        [Description("IDL Jobs File")]
        jobs,
        [Description("IDL Threads File")]
        threads,
        [Description("IDL Events File")]
        events,
        [Description("IDL Messages File")]
        msgs,
        [Description("IDL Notifications File")]
        notifs,
        [Description("IDL Alerts File")]
        alerts,
        [Description("IDL Warnings File")]
        warns,
        [Description("IDL Errors File")]
        errs,
        [Description("IDL Failures File")]
        fails,
        [Description("IDL Logs File")]
        logs,
        [Description("IDL Traces File")]
        traces,
        [Description("IDL Profiles File")]
        profs,
        [Description("IDL Tests File")]
        tests,
        [Description("IDL Benchmarks File")]
        benches,
        [Description("IDL Metrics File")]
        metrics,
        [Description("IDL Statistics File")]
        stats,
        [Description("IDL Results File")]
        results,
        [Description("IDL Outputs File")]
        outputs,
        [Description("IDL Analyses File")]
        analyses,
        [Description("IDL Insights File")]
        insights,
        [Description("IDL Findings File")]
        findings,
        [Description("IDL Conclusions File")]
        concls,
        [Description("IDL Recommendations File")]
        recs,
        [Description("IDL Actions File")]
        actions,
        [Description("IDL Plans File")]
        plans,
        [Description("IDL Strategies File")]
        strats,
        [Description("IDL Tactics File")]
        tactics,
        [Description("IDL Approaches File")]
        approaches,
        [Description("IDL Techniques File")]
        techs,
        [Description("IDL Practices File")]
        practices,
        [Description("IDL Workflows File")]
        workflows,
        [Description("IDL Pipelines File")]
        pipes,
        [Description("IDL Stages File")]
        stages,
        [Description("IDL Steps File")]
        steps,
        [Description("IDL Phases File")]
        phases,
        [Description("IDL Milestones File")]
        miles,
        [Description("IDL Goals File")]
        goals,
        [Description("IDL Objectives File")]
        objs,
        [Description("IDL Targets File")]
        targets,
        [Description("IDL Outcomes File")]
        outcomes,
        [Description("IDL Deliverables File")]
        delivs,
        [Description("IDL Products File")]
        prods,
        [Description("IDL Services File")]
        servs,
        [Description("IDL Solutions File")]
        sols,
        [Description("IDL Offerings File")]
        offers,
        [Description("IDL Packages File")]
        packs,
        [Description("IDL Bundles File")]
        bundles,
        [Description("IDL Kits File")]
        kits,
        [Description("IDL Sets File")]
        sets,
        [Description("IDL Collections File")]
        colls,
        [Description("IDL Groups File")]
        groups,
        [Description("IDL Categories File")]
        cats,
        [Description("IDL Kinds File")]
        kinds,
        [Description("IDL Forms File")]
        forms,
        [Description("IDL Shapes File")]
        shapes,
        [Description("IDL Sizes File")]
        sizes,
        [Description("IDL Colors File")]
        colors,
        [Description("IDL Textures File")]
        textures,
        [Description("IDL Patterns File")]
        patterns,
        [Description("IDL Styles File")]
        styles,
        [Description("IDL Designs File")]
        designs,
        [Description("IDL Models File")]
        models,
        [Description("IDL Prototypes File")]
        protos,
        [Description("IDL Blueprints File")]
        blueprints,
        [Description("IDL Schematics File")]
        schems,
        [Description("IDL Diagrams File")]
        diags,
        [Description("IDL Charts File")]
        charts,
        [Description("IDL Graphs File")]
        graphs,
        [Description("IDL Maps File")]
        maps,
        [Description("IDL Layouts File")]
        layouts,
        [Description("IDL Drawings File")]
        drawings,
        [Description("IDL Sketches File")]
        sketches,
        [Description("IDL Illustrations File")]
        illus,
        [Description("IDL Images File")]
        imgs,
        [Description("IDL Photos File")]
        photos,
        [Description("IDL Pictures File")]
        pics,
        [Description("IDL Snapshots File")]
        snaps,
        [Description("IDL Screenshots File")]
        screens,
        [Description("IDL Icons File")]
        icons,
        [Description("IDL Logos File")]
        logos,
        [Description("IDL Symbols File")]
        symbols,
        [Description("IDL Signs File")]
        signs,
        [Description("IDL Marks File")]
        marks,
        [Description("Shortcut File for URL")]
        url,
        [Description("Windows Shortcut File")]
        lnk,
    }
}
